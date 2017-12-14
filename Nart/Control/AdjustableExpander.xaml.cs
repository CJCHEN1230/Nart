﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Nart.Control
{
    /// <summary>
    /// AdjustableExpander.xaml 的互動邏輯
    /// </summary>
    public partial class AdjustableExpander : Expander
    {
        public AdjustableExpander()
        {
            InitializeComponent();
        }

        public Brush HeaderBackground
        {
            set
            {
                SetValue(HeaderBackgroundProperty, value);
            }
            get
            {
                return (Brush)GetValue(HeaderBackgroundProperty);
            }
        }
        public static readonly DependencyProperty HeaderBackgroundProperty = DependencyProperty.Register("HeaderBackground", typeof(Brush), typeof(AdjustableExpander), new UIPropertyMetadata(Brushes.Transparent));

        public Brush HeaderBorderBrush
        {
            set
            {
                SetValue(HeaderBorderBrushProperty, value);
            }
            get
            {
                return (Brush)GetValue(HeaderBorderBrushProperty);
            }
        }
        public static readonly DependencyProperty HeaderBorderBrushProperty = DependencyProperty.Register("HeaderBorderBrush", typeof(Brush), typeof(AdjustableExpander), new UIPropertyMetadata(Brushes.Transparent));

        public Thickness HeaderBorderThickness
        {
            set
            {
                SetValue(HeaderBorderThicknessProperty, value);
            }
            get
            {
                return (Thickness)GetValue(HeaderBorderThicknessProperty);
            }
        }
        public static readonly DependencyProperty HeaderBorderThicknessProperty = DependencyProperty.Register("HeaderBorderThickness", typeof(Thickness), typeof(AdjustableExpander), new UIPropertyMetadata(new Thickness(0, 0, 0, 0)));

        public Thickness ContentMargin
        {
            set
            {
                SetValue(ContentMarginProperty, value);
            }
            get
            {
                return (Thickness)GetValue(ContentMarginProperty);
            }
        }
        public static readonly DependencyProperty ContentMarginProperty = DependencyProperty.Register("ContentMargin", typeof(Thickness), typeof(AdjustableExpander), new UIPropertyMetadata(new Thickness(0, 0, 0, 0)));

        public CornerRadius HeaderCornerRadius
        {
            set
            {
                SetValue(HeaderCornerRadiusProperty, value);
            }
            get
            {
                return (CornerRadius)GetValue(HeaderCornerRadiusProperty);
            }
        }
        public static readonly DependencyProperty HeaderCornerRadiusProperty = DependencyProperty.Register("HeaderCornerRadius", typeof(CornerRadius), typeof(AdjustableExpander), new UIPropertyMetadata(new CornerRadius(0)));

        public Brush ArrowColor
        {
            set
            {
                SetValue(ArrowColorProperty, value);
            }
            get
            {
                return (Brush)GetValue(ArrowColorProperty);
            }
        }
        public static readonly DependencyProperty ArrowColorProperty = DependencyProperty.Register("ArrowColorProperty", typeof(Brush), typeof(AdjustableExpander), new UIPropertyMetadata(Brushes.Transparent));

    }
}