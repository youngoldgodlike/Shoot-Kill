using UnityEngine;

namespace Enemies.DmgInfliction
{
    public static class LayerExtensions
    {
        public static int ToLayer (this LayerMask mask ) {
            var bitmask = mask;
            int result = bitmask>0 ? 0 : 31;
            while( bitmask>1 ) {
                bitmask = bitmask>>1;
                result++;
            }
            return result;
        }
    }
}