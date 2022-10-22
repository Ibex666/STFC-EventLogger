using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Effects.Shader
{
    public class EmbossEffect : ShaderEffectBase
    {
        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }
        public static readonly DependencyProperty InputProperty =
            ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(EmbossEffect), 0);

        public double Strength
        {
            get { return (double)GetValue(StrengthProperty); }
            set { SetValue(StrengthProperty, value); }
        }
        public static readonly DependencyProperty StrengthProperty =
            DependencyProperty.Register("Strength", typeof(double), typeof(EmbossEffect), 
            new UIPropertyMetadata(2.0, PixelShaderConstantCallback(1)));
        
        public double Displacement
        {
            get { return (double)GetValue(DisplacementProperty); }
            set { SetValue(DisplacementProperty, value); }
        }
        public static readonly DependencyProperty DisplacementProperty =
            DependencyProperty.Register("Displacement", typeof(double), typeof(EmbossEffect), 
            new UIPropertyMetadata(0.002, PixelShaderConstantCallback(0)));

        protected override Uri GetPSUri()
        {
            return GetUri("Shader/EmbossEffect.ps", typeof(EmbossEffect));
        }

        protected override void UpdateShader()
        {
            UpdateShaderValue(InputProperty);
        }
    }
}
