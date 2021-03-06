/*
    This file is part of the iText (R) project.
    Copyright (c) 1998-2019 iText Group NV
    Authors: iText Software.

This program is free software; you can redistribute it and/or modify it under the terms of the GNU Affero General Public License version 3 as published by the Free Software Foundation with the addition of the following permission added to Section 15 as permitted in Section 7(a): FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY iText Group NV, iText Group NV DISCLAIMS THE WARRANTY OF NON INFRINGEMENT OF THIRD PARTY RIGHTS.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License along with this program; if not, see http://www.gnu.org/licenses or write to the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA, 02110-1301 USA, or download the license from the following URL:

http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions of this program must display Appropriate Legal Notices, as required under Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License, a covered work must retain the producer line in every PDF that is created or manipulated using iText.

You can be released from the requirements of the license by purchasing a commercial license. Buying such a license is mandatory as soon as you develop commercial activities involving the iText software without disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP, serving PDFs on the fly in a web application, shipping iText with a closed source product.

For more information, please contact iText Software Corp. at this address: sales@itextpdf.com */
using System;

using Org.BouncyCastle.Asn1.X509;

namespace Org.BouncyCastle.Asn1.Crmf
{
    public class PopoSigningKeyInput
        : Asn1Encodable
    {
        private readonly GeneralName            sender;
        private readonly PKMacValue             publicKeyMac;
        private readonly SubjectPublicKeyInfo   publicKey;

        private PopoSigningKeyInput(Asn1Sequence seq)
        {
            Asn1Encodable authInfo = (Asn1Encodable)seq[0];

            if (authInfo is Asn1TaggedObject)
            {
                Asn1TaggedObject tagObj = (Asn1TaggedObject)authInfo;
                if (tagObj.TagNo != 0)
                {
                    throw new ArgumentException("Unknown authInfo tag: " + tagObj.TagNo, "seq");
                }
                sender = GeneralName.GetInstance(tagObj.GetObject());
            }
            else
            {
                publicKeyMac = PKMacValue.GetInstance(authInfo);
            }

            publicKey = SubjectPublicKeyInfo.GetInstance(seq[1]);
        }

        public static PopoSigningKeyInput GetInstance(object obj)
        {
            if (obj is PopoSigningKeyInput)
                return (PopoSigningKeyInput)obj;

            if (obj is Asn1Sequence)
                return new PopoSigningKeyInput((Asn1Sequence)obj);

            throw new ArgumentException("Invalid object: " + obj.GetType().Name, "obj");
        }

        /** Creates a new PopoSigningKeyInput with sender name as authInfo. */
        public PopoSigningKeyInput(
            GeneralName sender,
            SubjectPublicKeyInfo spki)
        {
            this.sender = sender;
            this.publicKey = spki;
        }

        /** Creates a new PopoSigningKeyInput using password-based MAC. */
        public PopoSigningKeyInput(
            PKMacValue pkmac,
            SubjectPublicKeyInfo spki)
        {
            this.publicKeyMac = pkmac;
            this.publicKey = spki;
        }

        /** Returns the sender field, or null if authInfo is publicKeyMac */
        public virtual GeneralName Sender
        {
            get { return sender; }
        }

        /** Returns the publicKeyMac field, or null if authInfo is sender */
        public virtual PKMacValue PublicKeyMac
        {
            get { return publicKeyMac; }
        }

        public virtual SubjectPublicKeyInfo PublicKey
        {
            get { return publicKey; }
        }

        /**
         * <pre>
         * PopoSigningKeyInput ::= SEQUENCE {
         *        authInfo             CHOICE {
         *                                 sender              [0] GeneralName,
         *                                 -- used only if an authenticated identity has been
         *                                 -- established for the sender (e.g., a DN from a
         *                                 -- previously-issued and currently-valid certificate
         *                                 publicKeyMac        PKMacValue },
         *                                 -- used if no authenticated GeneralName currently exists for
         *                                 -- the sender; publicKeyMac contains a password-based MAC
         *                                 -- on the DER-encoded value of publicKey
         *        publicKey           SubjectPublicKeyInfo }  -- from CertTemplate
         * </pre>
         * @return a basic ASN.1 object representation.
         */
        public override Asn1Object ToAsn1Object()
        {
            Asn1EncodableVector v = new Asn1EncodableVector();

            if (sender != null)
            {
                v.Add(new DerTaggedObject(false, 0, sender));
            }
            else
            {
                v.Add(publicKeyMac);
            }

            v.Add(publicKey);

            return new DerSequence(v);
        }
    }
}
