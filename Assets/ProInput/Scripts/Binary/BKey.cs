using System;
using UnityEngine.InputSystem;

namespace ProInput.Scripts.Binary {
    /// <summary>
    /// Binary version of Key
    /// </summary>
    [Serializable]
    public class BKey {
        /// <summary>
        /// Current key value
        /// </summary>
        private string _value;
        
        /// <summary>
        /// Constructor for BKey
        /// </summary>
        /// <param name="key">Used to initialize BKey</param>
        public BKey(Key key) {
            _value = key.ToString();
        }

        /// <summary>
        /// Private key setter
        /// </summary>
        private Key Key {
            get {
                var worked = Enum.TryParse<Key>(_value, out var key);
                return worked ? key : Key.Backspace;
            }
            set => _value = value.ToString();
        }

        /// <summary>
        /// Convert Key to BKey
        /// </summary>
        /// <param name="bKey">BKey used for conversion</param>
        public static implicit operator Key(BKey bKey) {
            return bKey.Key;
        }

        /// <summary>
        /// Convert Key to BKey
        /// </summary>
        /// <param name="key">Key used for conversion</param>
        public static implicit operator BKey(Key key) {
            return new BKey(key);
        }
    }
}