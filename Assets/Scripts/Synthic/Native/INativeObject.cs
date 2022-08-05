namespace Synthic.Native
{
    public interface INativeObject
    {
        public bool Allocated { get; }
        internal void ReleaseResources();
    }
}