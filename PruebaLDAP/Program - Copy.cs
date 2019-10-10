using System;
using System.DirectoryServices.Protocols;
using System.Security.Cryptography;

namespace PruebaLDAP
{
    class Program2
    {
        public static bool validateUser(LdapConnection connection, string username, string password)
        {
            var request = new CompareRequest(username,
                "userPassword",
                password
                );
            var response = (CompareResponse)connection.SendRequest(request);
            return response.ResultCode == ResultCode.CompareTrue;
        }

        //var username = $"cn=ldap_usr,o={organization}";
        //var password = "usuldap";
        //var username = $"cn=solopez,ou=DDESARROL,ou=GSISTEMAS,ou=ADM,o={organization}";
        //var password = "1qaz2WSX";

        //var usuario = "lestrada";
        //var usuario = "solopez";
        //var clave = "1qaz2WSfX";
        //var username = $"cn=ldap_portal,ou=DDESARROL,ou=GSISTEMAS,ou=ADM,o={organization}";
        //var claveConexion = "G4rr4h4n19";



        /*
        Console.WriteLine($"Usuario {usuarioConexion} valida clave {validateUser(ldap, usuarioConexion, claveConexion)}");
        Console.WriteLine($"Usuario {usuario} valida clave {validateUser(ldap, $"cn={usuario},ou=UsuariosLDAP,o={organization}", clave)}");

        var searcher = new System.DirectoryServices.DirectorySearcher();
        searcher.Filter = String.Format("sAMAccountName={0}", usuario);
        searcher.SearchScope = System.DirectoryServices.SearchScope.Subtree;
        searcher.CacheResults = false;

        var searchResult = searcher.FindOne();
        if (searchResult == null)
        {
            Console.WriteLine($"Usuario {usuario} no encontrado");
        }

        var userEntry = searchResult.GetDirectoryEntry();
        Console.WriteLine($"Usuario encontrado: {usuario} - {userEntry.Properties["givenName"].Value}");
        Console.WriteLine($"clave {userEntry.Properties["userPassword"].Value}");
        Console.WriteLine("The Child ADS objects are:");
        foreach (System.DirectoryServices.DirectoryEntry myChildDirectoryEntry in userEntry.Children)
            Console.WriteLine(myChildDirectoryEntry.Path);

        Console.WriteLine("Prueba cambio de clave");

        userEntry.Properties["givenName"].Value = "hola";// $"\"{clave}2\"";
        userEntry.CommitChanges();
        */
        /*
        LdapAttribute attributePassword = new LdapAttribute("userPassword",
    newPassword);
        lc.Modify(modifyDN, new LdapModification(
            LdapModification.REPLACE, attributePassword));
            */
        /*
        var resultValueC = userEntry.Invoke("ChangePassword", new object[] { "1234Clave1", $"\"{clave}2\"" });
        userEntry.CommitChanges();
        Console.WriteLine($"cambio clave - resultValue: {resultValueC}");
        */

        /*
        var resultValue = userEntry.Invoke("SetPassword", new object[] { clave });
        userEntry.Properties["LockOutTime"].Value = 0;
        userEntry.Close();
        userEntry.CommitChanges();
        Console.WriteLine($"set clave - resultValue: {resultValue}");
        */

        /*
        //string newPassword = Membership.GeneratePassword(12, 4);
        string quotePwd = String.Format(@"""{0}""", clave);
        byte[] pwdBin = System.Text.Encoding.Unicode.GetBytes(quotePwd);
        userEntry.Properties["unicodePwd"].Value = pwdBin;
        userEntry.CommitChanges();
        Console.WriteLine($"set unicodePwd");
        */
        static void _Main(string[] args)
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
                var usuarioDN = $"cn={usuario},ou=UsuariosLDAP,o={organization}";

                using (var ldap = new LdapConnection(serverLdap))
                {
                    ldap.AuthType = AuthType.Basic;
                    ldap.SessionOptions.ProtocolVersion = 3;
                    ldap.AuthType = AuthType.Basic;
                    var credential = new System.Net.NetworkCredential(usuarioConexionDN, claveConexion);
                    ldap.Bind(credential);

                    var request = new CompareRequest(usuarioDN, "userPassword", clave);
                    var response = (CompareResponse)ldap.SendRequest(request);
                    var esClaveValida = response.ResultCode == ResultCode.CompareTrue 
                        ? "Valida"
                        : "Invalida";
                    Console.WriteLine($"La clave es: {esClaveValida}");

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
