﻿using System.Windows;
using System.Windows.Controls;

namespace ShiftManager.Controls
{
  public class PasswordStrengthVisualizerControl : Control
  {
    public string PasswordText { get => (string)GetValue(PasswordTextProperty); set => SetValue(PasswordTextProperty, value); }
    static public DependencyProperty PasswordTextProperty = DependencyProperty.Register(nameof(PasswordText), typeof(string), typeof(PasswordStrengthVisualizerControl), new(PWChanged));

    public double PasswordStrength { get => (double)GetValue(PasswordStrengthProperty); protected set => SetValue(PasswordStrengthPropertyKey, value); }
    static private DependencyPropertyKey PasswordStrengthPropertyKey = DependencyProperty.RegisterReadOnly(nameof(PasswordStrength), typeof(double), typeof(PasswordStrengthVisualizerControl), new());
    static public DependencyProperty PasswordStrengthProperty = PasswordStrengthPropertyKey.DependencyProperty;

    /// <summary>パスワードの強度における"良い"の閾値</summary>
    public static double PWStrength_Good { get; } = 0.7;
    /// <summary>パスワードの強度における"最低限"の閾値</summary>
    public static double PWStrength_Least { get; } = 0.4;

    static void PWChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) => (d as PasswordStrengthVisualizerControl).PasswordStrength = GetPasswordStrength(e.NewValue as string);

    static PasswordStrengthVisualizerControl() => DefaultStyleKeyProperty.OverrideMetadata(typeof(PasswordStrengthVisualizerControl), new FrameworkPropertyMetadata(typeof(PasswordStrengthVisualizerControl)));

    static public double GetPasswordStrength(in string s) => string.IsNullOrWhiteSpace(s) ? 0.1 : 0.2 + (double)Zxcvbn.Core.EvaluatePassword(s as string).Score / 5;//Scoreの最大値が4のため
  }
}
