using System.Collections.Generic;
using System.Xml.Serialization;

namespace Gir
{
    public class GRecord : GClass
    {
        #region Properties

        [XmlElement("field")]
        public List<GField> Fields { get; set; } = default!;

        [XmlAttribute("is-gtype-struct-for", Namespace = "http://www.gtk.org/introspection/glib/1.0")]
        public string? GLibIsGTypeStructFor;

        [XmlAttribute("disguised")]
        public bool Disguised;

        [XmlAttribute("introspectable")]
        public bool Introspectable = true;

        #endregion
    }
}
