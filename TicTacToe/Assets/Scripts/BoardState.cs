public class BoardState
{
    public int Width;
    public int Height;
    public FieldOwnerType LastActivePlayer;
    public FieldOwnerType[] FieldOwners;

    public BoardState Copy()
    {
        BoardState copy = new BoardState();
        copy.Width = Width;
        copy.Height = Height;
        copy.LastActivePlayer = LastActivePlayer;
        copy.FieldOwners = new FieldOwnerType[FieldOwners.Length];
        System.Array.Copy(FieldOwners, copy.FieldOwners, FieldOwners.Length);

        return copy;
    }
}
