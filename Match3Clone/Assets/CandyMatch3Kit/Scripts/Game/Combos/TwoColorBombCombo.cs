// Copyright (C) 2017 gamevanilla. All rights reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement,
// a copy of which is available at http://unity3d.com/company/legal/as_terms.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameVanilla.Core;

namespace GameVanilla.Game.Common
{
    /// <summary>
    /// The class used for the color bomb + color bomb combo.
    /// </summary>
    public class TwoColorBombCombo : ColorBombCombo
    {
        /// <summary>
        /// Resolves this combo.
        /// </summary>
        /// <param name="board">The game board.</param>
        /// <param name="tiles">The tiles destroyed by the combo.</param>
        /// <param name="fxPool">The pool to use for the visual effects.</param>
        public override void Resolve(GameBoard board, List<GameObject> tiles, FxPool fxPool)
        {
           //
            // Start coroutine with delay
            board.StartCoroutine(DelayedResolve(board, tiles, fxPool));
            base.Resolve(board, tiles, fxPool);
            SoundManager.instance.PlaySound("ColorBomb");
        }

        // Coroutine to handle the delay and explosion logic
        private IEnumerator DelayedResolve(GameBoard board, List<GameObject> tiles, FxPool fxPool)
        {
           
            yield return new WaitForSeconds(0.4f); 
            

            // Explode each matching tile
            for (var i = tiles.Count - 1; i >= 0; i--)
            {
                yield return new WaitForSeconds(0.03f);
                var tile = tiles[i];
                if (tile != null && (tile.GetComponent<Candy>() != null || tile.GetComponent<ColorBomb>() != null))
                {
                    
                    board.ExplodeTileNonRecursive(tile);
                }
            }

           

            board.ApplyGravity();
        }
    }
}
