using System.Security.Cryptography;

namespace Repostories.Utils
{
    public static class PasswordHash
    {
        private static readonly int SaltSize = 16; //tam salt
        private static readonly int HashSize = 20; //tam hash

        public static string CreateHash(string password, out string salt)//out porque o salt será gerado/retornado aqui
        {
            //gerar salt para que senhas iguais não tenham o mesmo hash
            using (var rng = new RNGCryptoServiceProvider()) //RNG gera números aleatórios
            {
                var saltBytes = new byte[SaltSize];
                rng.GetBytes(saltBytes);//array de bytes aleatórios
                salt = Convert.ToBase64String(saltBytes);//converte pra string

                //salt+senha=hash
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 10000))
                //10000 é o número de iterações, pbkdf2 é um algoritmo de hash
                {
                    var hashBytes = pbkdf2.GetBytes(HashSize);//gera hash
                    var hash = Convert.ToBase64String(hashBytes);//converte pra string
                    //salt+hash
                    return $"{salt}:{hash}";
                }
            }
        }
        public static bool VerifyPassword(string password, string storedHash)
        {
            //separa salt e hash pra verificar se a senha é a mesma
            var parts = storedHash.Split(':');
            if (parts.Length != 2) throw new InvalidOperationException("Formato de hash inválido");
            //se as partes separadas pelo : não forem 2, o hash é inválido

            var salt = parts[0];//antes do :
            var hash = parts[1];//depois do :

            //pega o hash com o salt pra comparar com o hash armazenado
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, Convert.FromBase64String(salt), 10000))
            //recupera o salt e converte pra bytes a partir da string/senha fornecida
            {
                var hashBytes = pbkdf2.GetBytes(HashSize);
                var newHash = Convert.ToBase64String(hashBytes);
                return newHash == hash;
            }
            //se a senha fornecida quando for hasheada com o salt for igual ao hash armazenado, a senha é a mesma

        }
    }
}
