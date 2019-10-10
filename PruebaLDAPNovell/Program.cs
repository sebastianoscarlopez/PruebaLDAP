/******************************************************************************
* The MIT License
* Copyright (c) 2006 Novell Inc.  www.novell.com
* 
* Permission is hereby granted, free of charge, to any person obtaining  a copy
* of this software and associated documentation files (the Software), to deal
* in the Software without restriction, including  without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
* copies of the Software, and to  permit persons to whom the Software is 
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in 
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED AS IS, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
*******************************************************************************/
//
// Samples.SetPassword.cs
//
// Author:
//   Palaniappan N (NPalaniappan@novell.com)
//
// (C) 2006 Novell, Inc (http://www.novell.com)
//

/*
*   The SetPassword.cs sample shows how to set the password
*   of an entry by setting the userPassword attribute
*   of the entry.
*
*   In Novell eDirectory, only an admin can set a password
*   without supplying the old password.  Consequently this
*   method works on any Novell Ldap server, but only when the
*   caller has admin privileges. 
*/


using Novell.Directory.Ldap;

using System;

public class SetPassword
{
    public static void Main(String[] args)
    {
        if (args.Length != 6)
        {
            Console.Error.WriteLine("Usage:   mono SetPassword <host name> "
                + "<login dn> <password>\n"
                + "         <modify dn> <new password>");
            Console.Error.WriteLine("Example: mono SetPassword Acme.com "
                + "\"cn=Admin,o=Acme secret\"\n"
                + "         \"cn=JSmith,ou=Sales,o=Acme\""
                + " newPassword");
            Environment.Exit(1);
        }
        switch (args[0])
        {
            /*
             * Ejemplo
             * CLAVE 172.16.10.82 cn=ldap_portal,ou=DDESARROL,ou=GSISTEMAS,ou=ADM,o=garrahan G4rr4h4n19 ldaptester1 12ClaveAB
             * */
            case "CLAVE":
                CambiarClave(args[1], args[2], args[3], args[4], args[0]);
                break;
            /*
             * Ejemplo
             * ALTA 172.16.10.82 cn=ldap_portal,ou=DDESARROL,ou=GSISTEMAS,ou=ADM,o=garrahan G4rr4h4n19 ldaptester3 12ClaveAB
             * */
            case "ALTA":
                Alta(args[1], args[2], args[3], args[4], args[0]);
                break;
        }
    }
    private static void Alta(string ldapHost, string conexionDN, string conexionClave, string usuario, string clave)
    {
        var groupdn = $"ou=UsuariosLDAP,o=garrahan";
        var userdn = $"cn={usuario},ou=UsuariosLDAP,o=garrahan";
        groupdn = userdn;

        int ldapPort = LdapConnection.DEFAULT_PORT;
        int ldapVersion = LdapConnection.Ldap_V3;
        LdapConnection lc = new LdapConnection();

        try
        {
            // connect to the server
            lc.Connect(ldapHost, ldapPort);
            lc.Bind(ldapVersion, conexionDN, conexionClave);

            //Creates the List attributes of the entry and add them to attribute set 
            LdapAttributeSet attributeSet = new LdapAttributeSet();
            attributeSet.Add(new LdapAttribute("objectclass", "inetOrgPerson"));
            attributeSet.Add(new LdapAttribute("cn", new string[] { $"{usuario} Prueba", $"Prueba {usuario}" }));
            attributeSet.Add(new LdapAttribute("givenname", "Prueba"));
            attributeSet.Add(new LdapAttribute("sn", $"SN{usuario}"));
            attributeSet.Add(new LdapAttribute("mail", $"{usuario}@correo.com"));

            // DN of the entry to be added
            string dn = userdn;
            LdapEntry newEntry = new LdapEntry(dn, attributeSet);

            //Add the entry to the directory
            lc.Add(newEntry);

            /*
            // modifications for group and user
            LdapModification[] modGroup = new LdapModification[2];
            LdapModification[] modUser = new LdapModification[2];

            // Add modifications to modUser
            LdapAttribute membership = new LdapAttribute("groupMembership", groupdn);
            modUser[0] = new LdapModification(LdapModification.ADD, membership);
            LdapAttribute security = new LdapAttribute("securityEquals", groupdn);
            modUser[1] = new LdapModification(LdapModification.ADD, security);

            // Add modifications to modGroup
            LdapAttribute member = new LdapAttribute("uniqueMember", userdn);
            modGroup[0] = new LdapModification(LdapModification.ADD, member);
            LdapAttribute equivalent = new LdapAttribute("equivalentToMe", userdn);
            modGroup[1] = new LdapModification(LdapModification.ADD, equivalent);

            try
            {
                // Modify the user's attributes
                lc.Modify(userdn, modUser);
                Console.Out.WriteLine("Modified the user's attribute.");
            }
            catch (LdapException e)
            {
                Console.Out.WriteLine("Failed to modify user's attributes: " + e.LdapErrorMessage);
            }

            try
            {
                // Modify the group's attributes
                lc.Modify(groupdn, modGroup);
                Console.Out.WriteLine("Modified the group's attribute.");
            }
            catch (LdapException e)
            {
                Console.Out.WriteLine("Failed to modify group's attributes: " + e.LdapErrorMessage);
                doCleanup(lc, userdn, groupdn);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:" + e.Message);
            }
        */
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.ToString());
        }
        Console.ReadKey();
        Environment.Exit(1);
    }
    public static void doCleanup(LdapConnection conn, String userdn, String groupdn)
    {
        // since we have modified the user's attributes and failed to
        // modify the group's attribute, we need to delete the modified
        // user's attribute values.

        // modifications for user
        LdapModification[] modUser = new LdapModification[2];

        // Delete the groupdn from the user's attributes
        LdapAttribute membership = new LdapAttribute("groupMembership", groupdn);
        modUser[0] = new LdapModification(LdapModification.DELETE, membership);
        LdapAttribute security = new LdapAttribute("securityEquals", groupdn);
        modUser[1] = new LdapModification(LdapModification.DELETE, security);

        try
        {
            // Modify the user's attributes
            conn.Modify(userdn, modUser);

            Console.Out.WriteLine("Deleted the modified user's attribute values.");
        }
        catch (LdapException e)
        {
            Console.Out.WriteLine("Could not delete modified user's attributes: " + e.LdapErrorMessage);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error:" + e.Message);
            return;
        }

        return;
    }

    private static void CambiarClave(string ldapHost, string conexionDN, string conexionClave, string usuario, string clave)
    {
        int ldapPort = LdapConnection.DEFAULT_PORT;
        int ldapVersion = LdapConnection.Ldap_V3;
        LdapConnection lc = new LdapConnection();

        try
        {
            // connect to the server
            lc.Connect(ldapHost, ldapPort);
            lc.Bind(ldapVersion, conexionDN, conexionClave);
            LdapSearchResults results = lc.Search("o=garrahan", LdapConnection.SCOPE_SUB, $"(&(objectClass=user)(uid={usuario}))", null, false, (LdapSearchConstraints)null);
            var hasResult = results.hasMore();
            if (hasResult)
            {
                var next = results.next();
                var usuarioDN = next.DN;
                var attributePassword = new LdapAttribute("userPassword",
                    clave);
                lc.Modify(usuarioDN, new LdapModification(
                    LdapModification.REPLACE, attributePassword));

                Console.WriteLine("Successfully set the user's password");
            }
            else
            {

            }
            lc.Disconnect();
        }
        catch (LdapException e)
        {
            if (e.ResultCode == LdapException.NO_SUCH_OBJECT)
            {
                Console.Error.WriteLine("Error: No such entry");
            }
            else if (e.ResultCode ==
                LdapException.INSUFFICIENT_ACCESS_RIGHTS)
            {
                Console.Error.WriteLine("Error: Insufficient rights");
            }
            else
            {
                Console.Error.WriteLine("Error: " + e.ToString());
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.ToString());
        }
        Console.ReadKey();
        Environment.Exit(1);
    }
}