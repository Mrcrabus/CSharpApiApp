namespace WebApplication1.Dto
{
    public interface ITokenDecode
    {
        string Id { get; }
        string UserName { get; }
        string Exp { get; }
        string TokenId { get; }
    }
}
