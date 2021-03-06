using System.Xml.Serialization;

namespace Gir
{
    public class GProperty
    {
        #region Properties

        [XmlAttribute("name")]
        public string? Name { get; set; }

        [XmlAttribute("writable")]
        public bool Writeable { get; set; }

        [XmlAttribute("construct")]
        public bool Construct { get; set; }

        [XmlAttribute("transfer-ownership")]
        public string? TransferOwnership { get; set; }

        [XmlAttribute("deprecated")]
        public bool Deprecated { get; set; }

        [XmlAttribute("deprecated-version")]
        public string? DeprecatedVersion { get; set; }

        [XmlElement("doc")]
        public GDoc? Doc { get; set; }

        [XmlElement("doc-deprecated")]
        public GDoc? DocDeprecated { get; set; }

        [XmlElement("type")]
        public GType? Type { get; set; }

        #endregion
    }
}
