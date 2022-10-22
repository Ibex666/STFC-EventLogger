using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Effects.Shader
{
    public class InvertEffect : ShaderEffectBase
    {
        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        public static readonly DependencyProperty InputProperty =
            ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(InvertEffect), 0);

        protected override Uri GetPSUri()
        {
            return GetUri("Shader/InvertEffect.ps", typeof(InvertEffect));
        }

        protected override void UpdateShader()
        {
            UpdateShaderValue(InputProperty);
        }
    }
}
