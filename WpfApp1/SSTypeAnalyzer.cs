﻿using STFC_EventLogger.AllianceClasses;
using System;
using System.Collections.Generic;
using System.Windows.Xps.Packaging;
using System.Xml;
using Tesseract;

namespace STFC_EventLogger
{
    internal class SSTypeAnalyzer : IEquatable<SSTypeAnalyzer?>
    {
        public SSTypeAnalyzer(string file, ImageTypes imageType)
        {
            FileName = file;
            ImageType = imageType;
        }

        public string FileName { get; set; }
        public PageTypes PageType { get; set; }
        public ImageTypes ImageType { get; set; }


        public void Analyze()
        {
            using var image = Pix.LoadFromFile(FileName);
            using var engine = new TesseractEngine(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"tessdata"), "eng", EngineMode.TesseractOnly);
            using var page = engine.Process(image, V.us.RectSsTypeAnalyzer);
            XmlDocument xdoc = new();
            xdoc.LoadXml(page.GetAltoText(0));
#pragma warning disable CS8602 // Dereferenzierung eines möglichen Nullverweises.
            var nodes = xdoc.SelectNodes("//@CONTENT");
            if (nodes != null && nodes.Count > 0 && nodes[0].Value != null && nodes[0].Value != null)
            {
                switch (nodes[0].Value.ToLower())
                {
                    case "members":
                        PageType = PageTypes.MemberList;
                        break;
                    default:
                        PageType = PageTypes.EventList;
                        break;
                }
            }
#pragma warning restore CS8602 // Dereferenzierung eines möglichen Nullverweises.
        }


        public override bool Equals(object? obj)
        {
            return Equals(obj as SSTypeAnalyzer);
        }

        public bool Equals(SSTypeAnalyzer? other)
        {
            return other is not null &&
                   FileName == other.FileName;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FileName);
        }

        public override string? ToString()
        {
            return $"{FileName}, {PageType}, {ImageType}";
        }

        public static bool operator ==(SSTypeAnalyzer? left, SSTypeAnalyzer? right)
        {
            return EqualityComparer<SSTypeAnalyzer>.Default.Equals(left, right);
        }

        public static bool operator !=(SSTypeAnalyzer? left, SSTypeAnalyzer? right)
        {
            return !(left == right);
        }
    }
}