using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

  public interface IPiece
{
    void Link_Child_Nodes();
    static IPiece Load(Player owner)
    {
        throw new NotImplementedException();
    }
    void Click(Player curr_player);
    void Set_Background();

    void Select();
    void Deselect();
}

