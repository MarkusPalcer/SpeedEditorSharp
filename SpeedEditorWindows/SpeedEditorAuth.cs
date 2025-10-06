using System;

namespace SpeedEditorWindows
{
    /// <summary>
    /// Authentication mechanism for the Blackmagic Speed Editor
    /// There is a mutual authentication mechanism where the software and the
    /// keyboard authenticate to each other... without that, the keyboard
    /// doesn't send REPORTs :/
    /// </summary>
    public static class SpeedEditorAuth
    {
        private static readonly ulong[] AUTH_EVEN_TBL = new ulong[]
        {
            0x3ae1206f97c10bc8,
            0x2a9ab32bebf244c6,
            0x20a6f8b8df9adf0a,
            0xaf80ece52cfc1719,
            0xec2ee2f7414fd151,
            0xb055adfd73344a15,
            0xa63d2e3059001187,
            0x751bf623f42e0dde,
        };

        private static readonly ulong[] AUTH_ODD_TBL = new ulong[]
        {
            0x3e22b34f502e7fde,
            0x24656b981875ab1c,
            0xa17f3456df7bf8c3,
            0x6df72e1941aef698,
            0x72226f011e66ab94,
            0x3831a3c606296b42,
            0xfd7ff81881332c89,
            0x61a3f6474ff236c6,
        };

        private const ulong MASK = 0xa79a63f585d37bf0;

        /// <summary>
        /// Rotate left by 8 bits
        /// </summary>
        private static ulong Rol8(ulong v)
        {
            return ((v << 56) | (v >> 8)) & 0xffffffffffffffff;
        }

        /// <summary>
        /// Rotate left by 8 bits n times
        /// </summary>
        private static ulong Rol8n(ulong v, int n)
        {
            for (int i = 0; i < n; i++)
            {
                v = Rol8(v);
            }
            return v;
        }

        /// <summary>
        /// Compute authentication response for the given challenge
        /// </summary>
        public static ulong ComputeAuthResponse(ulong challenge)
        {
            int n = (int)(challenge & 7);
            ulong v = Rol8n(challenge, n);

            ulong k;
            if ((v & 1) == ((0x78u >> n) & 1))
            {
                k = AUTH_EVEN_TBL[n];
            }
            else
            {
                v = v ^ Rol8(v);
                k = AUTH_ODD_TBL[n];
            }

            return v ^ (Rol8(v) & MASK) ^ k;
        }
    }
}