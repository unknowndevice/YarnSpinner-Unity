using System.Collections.Generic;
using UnityEngine;

namespace Yarn.Unity
{
    /// <summary>
    /// A <see cref="MonoBehaviour"/> that produces <see
    /// cref="LocalizedLine"/>s.
    /// </summary>
    /// <remarks>
    /// <see cref="DialogueRunner"/>s use a <see
    /// cref="LineProviderBehaviour"/> to get <see cref="LocalizedLine"/>s,
    /// which contain the localized information that <see
    /// cref="DialogueViewBase"/> classes use to present content to the
    /// player. 
    ///
    /// Subclasses of this abstract class may return subclasses of
    /// LocalizedLine. For example, <see cref="AudioLineProvider"/> returns
    /// an <see cref="AudioLocalizedLine"/>, which includes <see
    /// cref="AudioClip"/>; views that make use of audio can then access
    /// this additional data.
    /// </remarks>
    public abstract class LineProviderBehaviour : MonoBehaviour
    {
        /// <summary>
        /// The data source for this line provider.
        /// </summary>
        public LocalizationDatabase localizationDatabase;
        
        public string CurrentTextLanguageCode 
        { 
            get 
            { 
                return string.IsNullOrWhiteSpace(textLanguageCodeOverride) ? Preferences.TextLanguage : textLanguageCodeOverride;
            }
        }

        /// <summary>Specifies the language code to use for text content
        /// for this <see cref="LineProviderBehaviour"/>, overriding
        /// project settings.</summary>
        /// <remarks>
        /// If defined, this Line Provider will ignore the current setting
        /// in Preferences.TextLanguage and use the text language code
        /// override instead (e.g. "en" is the code for "English")
        /// </remarks>
        [Tooltip("(optional) if defined, this Line Provider will use this language code instead of Preferences.TextLanguage... example: 'en' is the code for English")]
        public string textLanguageCodeOverride;
        public abstract LocalizedLine GetLocalizedLine(Yarn.Line line);
        public abstract void PrepareForLines(IEnumerable<string> lineIDs);
        public abstract bool LinesAvailable {get;}

        public virtual void Start () {
            if ( !string.IsNullOrWhiteSpace(textLanguageCodeOverride) ) {
                Debug.LogWarning($"LineProvider is ignoring global Preferences.TextLanguage and using textLanguageCodeOverride: {textLanguageCodeOverride}");
            }
        }
    }

    /// <summary>
    /// Represents a line, ready to be presented to the user in the
    /// localisation they have specified.
    /// </summary>
    public class LocalizedLine {
        /// <summary>
        /// DialogueLine's ID
        /// </summary>
        public string TextID;
        /// <summary>
        /// DialogueLine's inline expression's substitution
        /// </summary>
        public string[] Substitutions;
        /// <summary>
        /// DialogueLine's text
        /// </summary>
        public string RawText;
        /// <summary>
        /// The line's delivery status.
        /// </summary>
        public LineStatus Status;

        /// <summary>
        /// The name of the character, if present.
        /// </summary>
        /// <remarks>
        /// This value will be <see langword="null"/> if the line does not
        /// have a character name.
        public string CharacterName {
            get {
                if (Text.TryGetAttributeWithName("character", out var characterNameAttribute)) {
                    if (characterNameAttribute.Properties.TryGetValue("name", out var value)) {
                        return value.StringValue;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// The underlying <see cref="Yarn.Markup.MarkupParseResult"/> for
        /// this line.
        /// </summary>
        public Markup.MarkupParseResult Text { get; internal set; }

        public Markup.MarkupParseResult TextWithoutCharacterName {
            get {
                // If a 'character' attribute is present, remove its text
                if (Text.TryGetAttributeWithName("character", out var characterNameAttribute)) {
                    return Text.DeleteRange(characterNameAttribute);                    
                } else {
                    return Text;
                }
            }
        }
    }

}
