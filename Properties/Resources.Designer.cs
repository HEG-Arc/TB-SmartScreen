﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré par un outil.
//     Version du runtime :4.0.30319.42000
//
//     Les modifications apportées à ce fichier peuvent provoquer un comportement incorrect et seront perdues si
//     le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

namespace POC_VoiceRecognition.Properties {
    using System;
    
    
    /// <summary>
    ///   Une classe de ressource fortement typée destinée, entre autres, à la consultation des chaînes localisées.
    /// </summary>
    // Cette classe a été générée automatiquement par la classe StronglyTypedResourceBuilder
    // à l'aide d'un outil, tel que ResGen ou Visual Studio.
    // Pour ajouter ou supprimer un membre, modifiez votre fichier .ResX, puis réexécutez ResGen
    // avec l'option /str ou régénérez votre projet VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Retourne l'instance ResourceManager mise en cache utilisée par cette classe.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("POC_VoiceRecognition.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Remplace la propriété CurrentUICulture du thread actuel pour toutes
        ///   les recherches de ressources à l'aide de cette classe de ressource fortement typée.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à Confidence level.
        /// </summary>
        internal static string ConfidenceLevel {
            get {
                return ResourceManager.GetString("ConfidenceLevel", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à The kinect is not ready or simply not available..
        /// </summary>
        internal static string kinectNotReady {
            get {
                return ResourceManager.GetString("kinectNotReady", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à No speech recongnizer found..
        /// </summary>
        internal static string NoSpeechRecognizer {
            get {
                return ResourceManager.GetString("NoSpeechRecognizer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Recherche une chaîne localisée semblable à &lt;grammar version=&quot;1.0&quot; xml:lang=&quot;en-US&quot; root=&quot;rootRule&quot; tag-format=&quot;semantics/1.0-literals&quot; xmlns=&quot;http://www.w3.org/2001/06/grammar&quot;&gt;
        ///  &lt;rule id=&quot;rootRule&quot;&gt;
        ///    &lt;one-of&gt;
        ///      &lt;item&gt;
        ///        &lt;tag&gt;FILTER_BY&lt;/tag&gt;
        ///        &lt;one-of&gt;
        ///          &lt;item&gt; filter by &lt;/item&gt;
        ///          &lt;item&gt; filter &lt;/item&gt;
        ///        &lt;/one-of&gt;
        ///      &lt;/item&gt;
        ///      &lt;item&gt;
        ///        &lt;tag&gt;ORDER_BY&lt;/tag&gt;
        ///        &lt;one-of&gt;
        ///          &lt;item&gt; order by &lt;/item&gt;
        ///          &lt;item&gt; order &lt;/item&gt;
        ///        &lt;/one-of&gt;
        ///      &lt;/item&gt;
        ///      &lt;item [le reste de la chaîne a été tronqué]&quot;;.
        /// </summary>
        internal static string SpeechGrammar {
            get {
                return ResourceManager.GetString("SpeechGrammar", resourceCulture);
            }
        }
    }
}
