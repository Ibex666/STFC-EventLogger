using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Effects.Shader
{
    public class GrayscaleEffect : ShaderEffectBase
    {
        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        public static readonly DependencyProperty InputProperty =
            ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(GrayscaleEffect), 0);

        protected override Uri GetPSUri()
        {
            return GetUri("Shader/GrayscaleEffect.ps", typeof(GrayscaleEffect));
        }

        protected override void UpdateShader()
        {
            UpdateShaderValue(InputProperty);
        }
    }
}
