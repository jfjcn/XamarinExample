namespace WebApiExamples.Domain.CDCollection
{
    public class Artist
    {
        public int Id { get; set; }

        public string Name { get; set; }









        public Artist() {}

        public Artist(Artist artistToCopy)
        {
            Id = artistToCopy.Id;
            Name = artistToCopy.Name;
        }

        protected bool Equals(Artist other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Artist) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public override string ToString()
        {
            return string.Format(
                "{0}:{1}",
                Id,
                Name);
        }
    }
}
