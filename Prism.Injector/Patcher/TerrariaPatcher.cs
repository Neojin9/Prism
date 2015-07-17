﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

namespace Prism.Injector.Patcher
{
    public static class TerrariaPatcher
    {
        static CecilContext   c;
        static MemberResolver r;

        static void PublicifyRec(TypeDefinition td)
        {
            if (!td.IsPublic)
                td.Attributes =
                    (td.Attributes & ~(td.Attributes & TypeAttributes.VisibilityMask)) | // remove all bits from the visibility mask (contains IsNested!)
                    (td.IsNested ? TypeAttributes.NestedPublic : TypeAttributes.Public); // set it to either Public (1) or NestedPublic (2), if it was nested before.

            foreach (FieldDefinition  d in td.Fields ) if (                !d.IsPublic) d.IsPublic = true;
            foreach (MethodDefinition d in td.Methods) if (!d.IsVirtual && !d.IsPublic) d.IsPublic = true; // don't change access modifier of overridden protected members etc

            if (td.HasNestedTypes)
                foreach (TypeDefinition d in td.NestedTypes)
                    PublicifyRec(d);
        }
        public static void Publicify()
        {
            // make all types and members in the "Terraria" namespace public

            foreach (TypeDefinition td in c.PrimaryAssembly.MainModule.Types)
            {
                if (td.Namespace != "Terraria")
                    continue;

                PublicifyRec(td);
            }
        }

        public static void Patch(CecilContext context, string outputPath)
        {
            c = context;
            r = c.Resolver;

            Publicify();

            // do other stuff here

            // Newtonsoft.Json.dll and Steamworks.NET.dll are required to write the assembly
            c.PrimaryAssembly.Write(outputPath);

            r = null;
            c = null;
        }
    }
}
