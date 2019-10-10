using System;
using System.DirectoryServices;
using System.DirectoryServices.Protocols;

namespace PruebaLDAP
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var serverLdap = "172.16.10.82";
                //var serverLdap = "172.16.10.42";
                //var serverLdap = "172.16.10.44:389";
                var organization = "garrahan";
                var usuarioConexionDN = $"cn=ldap_usr,o={organization}";
                var claveConexion = "usuldap";
                var usuario = "ldaptester1";
                var clave = "12ClaveAB";
                //var usuarioDN = $"cn={usuario},ou=UsuariosLDAP,o={organization}";

                using (var ldap = new LdapConnection(serverLdap))
                {
                    ldap.AuthType = AuthType.Basic;
                    ldap.SessionOptions.ProtocolVersion = 3;
                    ldap.AuthType = AuthType.Basic;
                    var credential = new System.Net.NetworkCredential(usuarioConexionDN, claveConexion);
                    ldap.Bind(credential);

                    var searcher = new SearchRequest
                    {
                        Filter = $"(&(objectClass=user)(uid={usuario}))"
                    };
                    var searchResult = (SearchResponse)ldap.SendRequest(searcher);
                    // Debe haber una unica entrada porque se supone que los usuarios son únicos sin importar en que lugar del arbol se encuentren
                    if (searchResult.Entries.Count == 1)
                    {
                        var usuarioDN =  searchResult.Entries[0].DistinguishedName;// "CN=soLopez,OU=DDESARROL,OU=GSISTEMAS,OU=ADM,O=GARRAHAN";//$"cn={usuario},ou=UsuariosLDAP,o={organization}";
                        var request = new CompareRequest(usuarioDN, "userPassword", clave);
                        var response = (CompareResponse)ldap.SendRequest(request);
                        var esClaveValida = response.ResultCode == ResultCode.CompareTrue
                            ? "Valida"
                            : "Invalida";
                        Console.WriteLine($"La clave es: {esClaveValida}");
                    }
                    else
                    {
                        Console.WriteLine($"Usuario no encontrado");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException?.Message ?? ex.Message);
            }
            Console.WriteLine("termino");
            Console.ReadKey();
        }
    }
}
