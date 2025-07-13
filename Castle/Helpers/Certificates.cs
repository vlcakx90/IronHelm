using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;

namespace Castle.Helpers
{
    public static class Certificates
    {
        public static X509Certificate2 GetSelfSigned(string name)
        {
            var PfxCertificate = Helpers.Certificates.CreateSelfSignedCert(name)
                .Export(X509ContentType.Pkcs12);

            return new X509Certificate2(PfxCertificate);
        }
        private static X509Certificate2 CreateSelfSignedCert(string name)
        {
            const int rsaSize = 2048;
            HashAlgorithmName hashAlgo = HashAlgorithmName.SHA256;

            using (var rsa = RSA.Create(rsaSize))
            {
                // Create request
                var distinguishedName = new X500DistinguishedName($"CN={name}");
                var request = new CertificateRequest(distinguishedName, rsa, hashAlgo, RSASignaturePadding.Pkcs1);

                // Extension
                var extension = new X509KeyUsageExtension(X509KeyUsageFlags.DataEncipherment | X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.DigitalSignature, false); // flags: https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.x509certificates.x509keyusageflags?view=net-8.0
                request.CertificateExtensions.Add(extension);

                // Enhanced Extension
                var enhancedExtension = new X509EnhancedKeyUsageExtension(new OidCollection { new("1.3.6.1.5.5.7.3.1") }, false); // serverAuth: https://oidref.com/1.3.6.1.5.5.7.3.1
                request.CertificateExtensions.Add(enhancedExtension);

                // Alt name
                var subjectAltName = new SubjectAlternativeNameBuilder();

                if (IPAddress.TryParse(name, out var address))
                {
                    subjectAltName.AddIpAddress(address);
                }
                else
                {
                    subjectAltName.AddDnsName(name);
                }

                request.CertificateExtensions.Add(subjectAltName.Build());

                // Create
                var notBefore = new DateTimeOffset(DateTime.UtcNow.AddDays(-1));
                var notAfter = new DateTimeOffset(DateTime.UtcNow.AddDays(100));
                X509Certificate2 cert = request.CreateSelfSigned(notBefore, notAfter);

                return cert;
            }            
        }

    }
}
