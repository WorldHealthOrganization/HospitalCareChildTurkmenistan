namespace who_pocket_book.Helpers
{
    public interface IImageResizer
    {
        byte[] Resize(byte[] original, int to);
    }
}