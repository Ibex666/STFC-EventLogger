using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Media.Effects;

namespace Effects.Shader
{
    public abstract class ShaderEffectBase : System.Windows.Media.Effects.ShaderEffect
    {
        protected static Uri GetUri(string filename, Type type)
        {
            Assembly assembly = type.Assembly;
            string assemblyName = assembly.ToString().Split(',')[0];
            string uriString = String.Format("pack://application:,,,/{0};component/{1}",
                assemblyName, filename);
            return new Uri(uriString);
        }

        protected abstract Uri GetPSUri();
        protected abstract void UpdateShader();

        public ShaderEffectBase()
        {
            var shader = new PixelShader();
            shader.UriSource = GetPSUri();

            PixelShader = shader;

            UpdateShader();
        }
    }
}
