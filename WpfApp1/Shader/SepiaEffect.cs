using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Effects.Shader
{
    public class SepiaEffect : ShaderEffectBase
    {
        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        public static readonly DependencyProperty InputProperty =
            ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(SepiaEffect), 0);

        protected override Uri GetPSUri()
        {
            return GetUri("Shader/SepiaEffect.ps", typeof(SepiaEffect));
        }

        protected override void UpdateShader()
        {
            UpdateShaderValue(InputProperty);
        }
    }
}
