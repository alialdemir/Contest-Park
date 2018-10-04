using Android.Content;
using Android.Graphics;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Xamarin.Forms;
using AApplication = Android.App.Application;

namespace ContestPark.Mobile.Droid.Extensions
{
    /// <summary>
    /// Interface of TypefaceCaches
    /// </summary>
    public interface ITypefaceCache
    {
        /// <summary>
        /// Removes typeface from cache
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="typeface">Typeface.</param>
        void StoreTypeface(string key, Typeface typeface);

        /// <summary>
        /// Removes the typeface.
        /// </summary>
        /// <param name="key">The key.</param>
        void RemoveTypeface(string key);

        /// <summary>
        /// Retrieves the typeface.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>Typeface.</returns>
        Typeface RetrieveTypeface(string key);
    }

    /// <summary>
    /// TypefaceCache caches used typefaces for performance and memory reasons.
    /// Typeface cache is singleton shared through execution of the application.
    /// You can replace default implementation of the cache by implementing ITypefaceCache
    /// interface and setting instance of your cache to static property SharedCache of this class
    /// </summary>
    public static class TypefaceCache
    {
        private static ITypefaceCache sharedCache;

        /// <summary>
        /// Returns the shared typeface cache.
        /// </summary>
        /// <value>The shared cache.</value>
        public static ITypefaceCache SharedCache
        {
            get
            {
                if (sharedCache == null)
                {
                    sharedCache = new DefaultTypefaceCache();
                }
                return sharedCache;
            }
            set
            {
                if (sharedCache != null && sharedCache.GetType() == typeof(DefaultTypefaceCache))
                {
                    ((DefaultTypefaceCache)sharedCache).PurgeCache();
                }
                sharedCache = value;
            }
        }
    }

    /// <summary>
    /// Default implementation of the typeface cache.
    /// </summary>
    internal class DefaultTypefaceCache : ITypefaceCache
    {
        private Dictionary<string, Typeface> _cacheDict;

        public DefaultTypefaceCache()
        {
            _cacheDict = new Dictionary<string, Typeface>();
        }

        public Typeface RetrieveTypeface(string key)
        {
            if (_cacheDict.ContainsKey(key))
            {
                return _cacheDict[key];
            }
            else
            {
                return null;
            }
        }

        public void StoreTypeface(string key, Typeface typeface)
        {
            _cacheDict[key] = typeface;
        }

        public void RemoveTypeface(string key)
        {
            _cacheDict.Remove(key);
        }

        public void PurgeCache()
        {
            _cacheDict = new Dictionary<string, Typeface>();
        }
    }

    /// <summary>
    /// Andorid specific extensions for Font class.
    /// </summary>
    public static class FontExtensions
    {
        /// <summary>
        /// This method returns typeface for given typeface using following rules:
        /// 1. Lookup in the cache
        /// 2. If not found, look in the assets in the fonts folder. Save your font under its FontFamily name.
        /// If no extension is written in the family name .ttf is asumed
        /// 3. If not found look in the files under fonts/ folder
        /// If no extension is written in the family name .ttf is asumed
        /// 4. If not found, try to return typeface from Xamarin.Forms ToTypeface() method
        /// 5. If not successfull, return Typeface.Default
        /// </summary>
        /// <returns>The extended typeface.</returns>
        /// <param name="font">Font</param>
        /// <param name="context">Android Context</param>
        public static Typeface ToExtendedTypeface(this Font font, Context context)
        {
            Typeface typeface = null;

            //1. Lookup in the cache
            var hashKey = font.ToHasmapKey();
            typeface = TypefaceCache.SharedCache.RetrieveTypeface(hashKey);
#if DEBUG
            if (typeface != null)
                Console.WriteLine("Typeface for font {0} found in cache", font);
#endif

            //2. If not found, try custom asset folder
            if (typeface == null && !string.IsNullOrEmpty(font.FontFamily))
            {
                string filename = font.FontFamily;
                //if no extension given then assume and add .ttf
                if (filename.LastIndexOf(".", System.StringComparison.Ordinal) != filename.Length - 4)
                {
                    filename = string.Format("{0}.ttf", filename);
                }
                try
                {
                    var path = "fonts/" + filename;
#if DEBUG
                    Console.WriteLine("Lookking for font file: {0}", path);
#endif
                    typeface = Typeface.CreateFromAsset(context.Assets, path);
#if DEBUG
                    Console.WriteLine("Found in assets and cached.");
#endif
#pragma warning disable CS0168 // Variable is declared but never used
                }
                catch (Exception ex)
                {
#if DEBUG
                    Console.WriteLine("not found in assets. Exception: {0}", ex);
                    Console.WriteLine("Trying creation from file");
#endif
                    try
                    {
                        typeface = Typeface.CreateFromFile("fonts/" + filename);

#if DEBUG
                        Console.WriteLine("Found in file and cached.");
#endif
                    }
                    catch (Exception ex1)
#pragma warning restore CS0168 // Variable is declared but never used
                    {
#if DEBUG
                        Console.WriteLine("not found by file. Exception: {0}", ex1);
                        Console.WriteLine("Trying creation using Xamarin.Forms implementation");
#endif
                    }
                }
            }
            //3. If not found, fall back to default Xamarin.Forms implementation to load system font
            if (typeface == null)
            {
                typeface = font.ToTypeface();
            }

            if (typeface == null)
            {
#if DEBUG
                Console.WriteLine("Falling back to default typeface");
#endif
                typeface = Typeface.Default;
            }
            //Store in cache
            TypefaceCache.SharedCache.StoreTypeface(hashKey, typeface);

            return typeface;
        }

        /// <summary>
        /// Provides unique identifier for the given font.
        /// </summary>
        /// <returns>Unique string identifier for the given font</returns>
        /// <param name="font">Font.</param>
        private static string ToHasmapKey(this Font font)
        {
            return string.Format("{0}.{1}.{2}.{3}", font.FontFamily, font.FontSize, font.NamedSize, (int)font.FontAttributes);
        }

        private static readonly Dictionary<Tuple<string, FontAttributes>, Typeface> _typefaces = new Dictionary<Tuple<string, FontAttributes>, Typeface>();

        // We don't create and cache a Regex object here because we may not ever need it, and creating Regexes is surprisingly expensive (especially on older hardware)
        // Instead, we'll use the static Regex.IsMatch below, which will create and cache the regex internally as needed. It's the equivalent of Lazy<Regex> with less code.
        // See https://msdn.microsoft.com/en-us/library/sdx2bds0(v=vs.110).aspx#Anchor_2
        private const string _loadFromAssetsRegex = @"\w+\.((ttf)|(otf))\#\w*";

        private static Typeface _defaultTypeface;

        public static float ToScaledPixel(this Font self)
        {
            if (self.IsDefault)
                return 14;

            if (self.UseNamedSize)
            {
                switch (self.NamedSize)
                {
                    case NamedSize.Micro:
                        return 10;

                    case NamedSize.Small:
                        return 12;

                    case NamedSize.Default:
                    case NamedSize.Medium:
                        return 14;

                    case NamedSize.Large:
                        return 18;
                }
            }

            return (float)self.FontSize;
        }

        public static Typeface ToTypeface(this Font self)
        {
            if (self.IsDefault)
                return _defaultTypeface ?? (_defaultTypeface = Typeface.Default);

            var key = new Tuple<string, FontAttributes>(self.FontFamily, self.FontAttributes);
            Typeface result;
            if (_typefaces.TryGetValue(key, out result))
                return result;

            if (self.FontFamily == null)
            {
                var style = ToTypefaceStyle(self.FontAttributes);
                result = Typeface.Create(Typeface.Default, style);
            }
            else if (Regex.IsMatch(self.FontFamily, _loadFromAssetsRegex))
            {
                result = Typeface.CreateFromAsset(AApplication.Context.Assets, FontNameToFontFile(self.FontFamily));
            }
            else
            {
                var style = ToTypefaceStyle(self.FontAttributes);
                result = Typeface.Create(self.FontFamily, style);
            }
            return (_typefaces[key] = result);
        }

        internal static bool IsDefault(this Entry self)
        {
            return self.FontFamily == null && self.FontAttributes == FontAttributes.None;
        }

        internal static Typeface ToTypeface(this Entry self)
        {
            if (self.IsDefault())
                return _defaultTypeface ?? (_defaultTypeface = Typeface.Default);

            var key = new Tuple<string, FontAttributes>(self.FontFamily, self.FontAttributes);
            Typeface result;
            if (_typefaces.TryGetValue(key, out result))
                return result;

            if (self.FontFamily == null)
            {
                var style = ToTypefaceStyle(self.FontAttributes);
                result = Typeface.Create(Typeface.Default, style);
            }
            else if (Regex.IsMatch(self.FontFamily, _loadFromAssetsRegex))
            {
                result = Typeface.CreateFromAsset(AApplication.Context.Assets, FontNameToFontFile(self.FontFamily));
            }
            else
            {
                var style = ToTypefaceStyle(self.FontAttributes);
                result = Typeface.Create(self.FontFamily, style);
            }
            return (_typefaces[key] = result);
        }

        public static TypefaceStyle ToTypefaceStyle(FontAttributes attrs)
        {
            var style = TypefaceStyle.Normal;
            if ((attrs & (FontAttributes.Bold | FontAttributes.Italic)) == (FontAttributes.Bold | FontAttributes.Italic))
                style = TypefaceStyle.BoldItalic;
            else if ((attrs & FontAttributes.Bold) != 0)
                style = TypefaceStyle.Bold;
            else if ((attrs & FontAttributes.Italic) != 0)
                style = TypefaceStyle.Italic;
            return style;
        }

        private static string FontNameToFontFile(string fontFamily)
        {
            int hashtagIndex = fontFamily.IndexOf('#');
            if (hashtagIndex >= 0)
                return fontFamily.Substring(0, hashtagIndex);

            throw new InvalidOperationException($"Can't parse the {nameof(fontFamily)} {fontFamily}");
        }
    }
}