using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameVanilla.Core;

namespace GameVanilla.Game.Common
{
    /// <summary>
    /// The class used for the color bomb + wrapped candy combo.
    /// </summary>
    public class ColorBombWithWrappedCandyCombo : ColorBombCombo
    {
        /// <summary>
        /// Resolves this combo.
        /// </summary>
        /// <param name="board">The game board.</param>
        /// <param name="tiles">The tiles destroyed by the combo.</param>
        /// <param name="fxPool">The pool to use for the visual effects.</param>
        public override void Resolve(GameBoard board, List<GameObject> tiles, FxPool fxPool)
        {
            board.StartCoroutine(ResolveWithDelay(board, tiles, fxPool));
        }

        private IEnumerator ResolveWithDelay(GameBoard board, List<GameObject> tiles, FxPool fxPool)
        {
            base.Resolve(board, tiles, fxPool);
            yield return new WaitForSeconds(2f);

            var wrapped = tileA.GetComponent<WrappedCandy>() != null ? tileA : tileB;

            var newTiles = new List<GameObject>();
            for (var i = tiles.Count - 1; i >= 0; i--)
            {
                var tile = tiles[i];
                if (tile != null && tile.GetComponent<Candy>() != null &&
                    tile.GetComponent<Candy>().color == wrapped.GetComponent<Candy>().color)
                {
                    var x = tile.GetComponent<Tile>().x;
                    var y = tile.GetComponent<Tile>().y;
                    board.ExplodeTileNonRecursive(tile);
                    var newTile = board.CreateWrappedTile(x, y, wrapped.GetComponent<Candy>().color);
                    newTiles.Add(newTile);
                }
                yield return new WaitForSeconds(0.04f);
            }

            SoundManager.instance.PlaySound("ColorBomb");

           

            board.ExplodeGeneratedTiles(newTiles);
        }
    }
}
