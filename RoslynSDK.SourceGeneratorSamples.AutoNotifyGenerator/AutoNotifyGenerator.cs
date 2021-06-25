﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

#pragma warning disable RS1024
#pragma warning disable IDE0007, IDE0057, IDE0060, IDE0062

namespace SourceGeneratorSamples
{
    /// <summary>INotifyPropertyChangedによる更新通知実装を容易にします.  開発支援用機能です.</summary>
    [Generator]
    public class AutoNotifyGenerator : ISourceGenerator
    {
        private const string attributeText = @"
#nullable disable
using System;
namespace AutoNotify
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    [System.Diagnostics.Conditional(""AutoNotifyGenerator_DEBUG"")]
    sealed class AutoNotifyAttribute : Attribute
    {
        public AutoNotifyAttribute()
        {
        }
        public string PropertyName { get; set; }
    }
}
";

        /*******************************************
        * specification ;
        * name = AutoNotifyGenerator.Initialize;
        * Function = コード自動生成機能の初期設定を行う;
        * note = OSSのコードであり, MITライセンスの下使用している ;
        * date = 02/26/2021 ;
        * author = Chris Sienkiewicz ( https://github.com/chsienki ) ;
        * History = (略) ;
        * input = SourceGeneratorの初期化に使用する構造体(GeneratorInitializationContext) ;
        * output = None;
        * end of specification ;
        *******************************************/
        public void Initialize(GeneratorInitializationContext context)
        {
            // Register the attribute source
            context.RegisterForPostInitialization((i) => i.AddSource("AutoNotifyAttribute", attributeText));

            // Register a syntax receiver that will be created for each generation pass
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        /*******************************************
        * specification ;
        * name = AutoNotifyGenerator.Execute ;
        * Function = ソースファイル(ソースコード)の自動生成を行う ;
        * note = OSSのコードであり, MITライセンスの下使用している ;
        * date = 02/26/2021 ;
        * author = Chris Sienkiewicz ( https://github.com/chsienki ) ;
        * History = (略) ;
        * input = SourceGenerator実行に使用する情報等が格納された構造体(GeneratorExecutionContext) ;
        * output = 自動生成されたソースファイル ;
        * end of specification ;
        *******************************************/
        public void Execute(GeneratorExecutionContext context)
        {
            // retrieve the populated receiver 
            if (!(context.SyntaxContextReceiver is SyntaxReceiver receiver))
                return;

            // get the added attribute, and INotifyPropertyChanged
            INamedTypeSymbol attributeSymbol = context.Compilation.GetTypeByMetadataName("AutoNotify.AutoNotifyAttribute");
            INamedTypeSymbol notifySymbol = context.Compilation.GetTypeByMetadataName("System.ComponentModel.INotifyPropertyChanged");

            // group the fields by class, and generate the source
            foreach (IGrouping<INamedTypeSymbol, IFieldSymbol> group in receiver.Fields.GroupBy(f => f.ContainingType))
            {
                string classSource = ProcessClass(group.Key, group.ToList(), attributeSymbol, notifySymbol, context);
                context.AddSource($"{group.Key.Name}_autoNotify.cs", SourceText.From(classSource, Encoding.UTF8));
            }
        }

        /*******************************************
        * specification ;
        * name = AutoNotifyGenerator.ProcessClass ;
        * Function = ソースコードの自動生成を行う ;
        * note = OSSのコードであり, MITライセンスの下使用している ;
        * date = 09/15/2020 ;
        * author = Chris Sienkiewicz ( https://github.com/chsienki ) ;
        * History = N/A ;
        * input = クラス情報 ;
        *       = フィールド情報リスト ;
        *       = 属性(Attribute)情報 ;
        *       = INotifyPropertyChanged interfaceの情報 ;
        *       = SourceGenerator実行に使用する情報等が格納された構造体 ;
        * output = 自動生成されたソースコード ;
        * end of specification ;
        *******************************************/
        private string ProcessClass(INamedTypeSymbol classSymbol, List<IFieldSymbol> fields, ISymbol attributeSymbol, ISymbol notifySymbol, GeneratorExecutionContext context)
        {
            if (!classSymbol.ContainingSymbol.Equals(classSymbol.ContainingNamespace, SymbolEqualityComparer.Default))
            {
                return null; //TODO: issue a diagnostic that it must be top level
            }

            string namespaceName = classSymbol.ContainingNamespace.ToDisplayString();

            // begin building the generated source
            StringBuilder source = new StringBuilder($@"
namespace {namespaceName}
{{
    public partial class {classSymbol.Name} : {notifySymbol.ToDisplayString()}
    {{
");

            // if the class doesn't implement INotifyPropertyChanged already, add it
            if (!classSymbol.Interfaces.Contains(notifySymbol))
            {
                source.Append("public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;");
            }

            // create properties for each field 
            foreach (IFieldSymbol fieldSymbol in fields)
            {
                ProcessField(source, fieldSymbol, attributeSymbol);
            }

            source.Append("} }");
            return source.ToString();
        }

        /*******************************************
        * specification ;
        * name = AutoNotifyGenerator.ProcessField ;
        * Function = フィールド情報から更新通知機能付きのプロパティを実現するソースコードを生成する ;
        * note = OSSのコードであり, MITライセンスの下使用している ;
        * date = 09/15/2020 ;
        * author = Chris Sienkiewicz ( https://github.com/chsienki ) ;
        * History = N/A ;
        * input = 出力するソースコードを記録するクラス ;
        *       = フィールド情報 ;
        *       = 属性(Attribute)情報 ;
        * output = 自動生成されたソースコード ;
        * end of specification ;
        *******************************************/
        private void ProcessField(StringBuilder source, IFieldSymbol fieldSymbol, ISymbol attributeSymbol)
        {
            // get the name and type of the field
            string fieldName = fieldSymbol.Name;
            ITypeSymbol fieldType = fieldSymbol.Type;

            // get the AutoNotify attribute from the field, and any associated data
            AttributeData attributeData = fieldSymbol.GetAttributes().Single(ad => ad.AttributeClass.Equals(attributeSymbol, SymbolEqualityComparer.Default));
            TypedConstant overridenNameOpt = attributeData.NamedArguments.SingleOrDefault(kvp => kvp.Key == "PropertyName").Value;

            string propertyName = chooseName(fieldName, overridenNameOpt);
            if (propertyName.Length == 0 || propertyName == fieldName)
            {
                //TODO: issue a diagnostic that we can't process this field
                return;
            }

            source.Append($@"
public {fieldType} {propertyName} 
{{
    get 
    {{
        return this.{fieldName};
    }}

    set
    {{
        this.{fieldName} = value;
        this.PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof({propertyName})));
    }}
}}

");

            /*******************************************
            * specification ;
            * name = AutoNotifyGenerator.ProcessField().chooseName (ローカル関数) ;
            * Function = 生成するプロパティ名を, プロパティ名から作成する ;
            * note = OSSのコードであり, MITライセンスの下使用している ;
            * date = 04/29/2020 ;
            * author = Chris Sienkiewicz ( https://github.com/chsienki ) ;
            * History = N/A ;
            * input = フィールド名 ;
            *       = 命名規則の設定(オプション) ;
            * output = 使用するプロパティ名 ;
            * end of specification ;
            *******************************************/
            string chooseName(string fieldName, TypedConstant overridenNameOpt)
            {
                if (!overridenNameOpt.IsNull)
                {
                    return overridenNameOpt.Value.ToString();
                }

                fieldName = fieldName.TrimStart('_');
                if (fieldName.Length == 0)
                    return string.Empty;

                if (fieldName.Length == 1)
                    return fieldName.ToUpper();

                return fieldName.Substring(0, 1).ToUpper() + fieldName.Substring(1);
            }

        }

        /// <summary>
        /// Created on demand before each generation pass
        /// </summary>
        class SyntaxReceiver : ISyntaxContextReceiver
        {
            public List<IFieldSymbol> Fields { get; } = new List<IFieldSymbol>();

            /// <summary>
            /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
            /// </summary>
            /*******************************************
            * specification ;
            * name = AutoNotifyGenerator.SyntaxReceiver.ProcessClass ;
            * Function = ソースコードの自動生成のために事前の情報収集を行う ;
            * note = OSSのコードであり, MITライセンスの下使用している ;
            * date = 02/26/2021 ;
            * author = Chris Sienkiewicz ( https://github.com/chsienki ) ;
            * History = N/A ;
            * input = ソースコードを自動生成する対象の情報 ;
            * output = None ;
            * end of specification ;
            *******************************************/
            public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
            {
                // any field with at least one attribute is a candidate for property generation
                if (context.Node is FieldDeclarationSyntax fieldDeclarationSyntax
                    && fieldDeclarationSyntax.AttributeLists.Count > 0)
                {
                    foreach (VariableDeclaratorSyntax variable in fieldDeclarationSyntax.Declaration.Variables)
                    {
                        // Get the symbol being declared by the field, and keep it if its annotated
                        IFieldSymbol fieldSymbol = context.SemanticModel.GetDeclaredSymbol(variable) as IFieldSymbol;
                        if (fieldSymbol.GetAttributes().Any(ad => ad.AttributeClass.ToDisplayString() == "AutoNotify.AutoNotifyAttribute"))
                        {
                            Fields.Add(fieldSymbol);
                        }
                    }
                }
            }
        }
    }
}
