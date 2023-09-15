using Microsoft.EntityFrameworkCore;

namespace URlShortener.Services
{
    public class UrlShortenServices
    {
        private readonly ApplicationDbContext _context;

        public UrlShortenServices(ApplicationDbContext context)
        {
            _context = context;
        }

        public const int NumberOfCharsInShortUrl = 7;
        private const string Alphabet = "ABCDEFGHIKLMNOPQRSTUVWXYZabcdfghiklmnopqrstuvwxyz0123456789";
        private readonly Random _random = new Random();

        public async Task<string> UniqueCode()
        {
           while (true)
            {
                //get array of char with 7 length
                var codeChar = new char[NumberOfCharsInShortUrl];
                for (var i = 0; i < NumberOfCharsInShortUrl; i++)
                {
                    var AlphabetIdx = _random.Next(Alphabet.Length - 1);
                    codeChar[i] = Alphabet[AlphabetIdx];
                }
                var code = new string(codeChar);
                if (!await _context.ShortenedUrls.AnyAsync(x => x.Code == code))
                {
                    return code;
                }
            }
        }
    }
}
