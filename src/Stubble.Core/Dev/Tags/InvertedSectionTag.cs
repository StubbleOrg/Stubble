namespace Stubble.Core.Dev.Tags
{
    /// <summary>
    /// A block tag representing an inverted section token
    /// </summary>
    public class InvertedSectionTag : BlockTag<InvertedSectionTag>
    {
        /// <summary>
        /// Gets or sets the sections name
        /// </summary>
        public string SectionName { get; set; }

        /// <summary>
        /// Gets or sets the starting position of the tag
        /// </summary>
        public int StartPosition { get; set; }

        /// <summary>
        /// Gets or sets the end position of the tag
        /// </summary>
        public int EndPosition { get; set; }

        /// <inheritdoc/>
        public override string Identifier => SectionName;

        /// <inheritdoc/>
        public override bool Equals(InvertedSectionTag other)
        {
            if (other == null)
            {
                return false;
            }

            if (Children != null && other.Children != null)
            {
                if (Children.Count != other.Children.Count)
                {
                    return false;
                }

                for (var i = 0; i < Children.Count; i++)
                {
                    var equal = other.Children[i].Equals(Children[i]);
                    if (!equal)
                    {
                        return false;
                    }
                }
            }

            return !(Children == null & other.Children != null) &&
                   !(Children != null & other.Children == null) &&
                   other.IsClosed == IsClosed &&
                   other.SectionName == SectionName &&
                   other.StartPosition == StartPosition &&
                   other.EndPosition == EndPosition;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            var a = obj as InvertedSectionTag;
            return a != null && Equals(a);
        }

        /// <summary>
        /// Gets the hash code for the tag
        /// </summary>
        /// <returns>The hashcode</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ StartPosition;
                hashCode = (hashCode * 397) ^ EndPosition;
                hashCode = (hashCode * 397) ^ (SectionName?.GetHashCode() ?? 0);
                return hashCode;
            }
        }
    }
}
