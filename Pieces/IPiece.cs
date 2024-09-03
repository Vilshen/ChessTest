using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Godot;

  public interface IPiece
{
    Sprite2D background_node { get; set; }
    Sprite2D piece_sprite { get; set; }
    bool has_moved { get; set; }
    public bool selected { get; set; }
    public bool attacked { get; set; } //only used for ~Kings, not checked for other pieces
    Player owner_player { get; }
    Vector2I board_position { get; set; }
    void Link_Child_Nodes();
    static IPiece Load(Player owner)
    {
        throw new NotImplementedException();
    }
    virtual void Click(Player curr_player)
    {
        if (owner_player.selected_piece == this)
        {
            this.Deselect();
        }
        else
        {
            this.Select();
        }
    }
    virtual void Set_Background()
    {
        if (selected)
        {
            background_node.Show();
            background_node.Texture = GD.Load<Texture2D>("res://assets/Background_Blue.png");
        }
        else if (attacked)
        {
            background_node.Show();
            background_node.Texture = GD.Load<Texture2D>("res://assets/Background_Red.png");
        }
        else
        {
            background_node.Hide();
        }
    }

    virtual void Select()
    {
        selected = true;
        if (owner_player.selected_piece is not null)
        {
            owner_player.selected_piece.Deselect();
        }
        owner_player.selected_piece = this;
        Set_Background();
    }
    virtual void Deselect()
    {
        selected = false;
        owner_player.selected_piece = null;
        Set_Background();
    }

    (List<Vector2I>, List<Vector2I>) All_Destinations(IPiece[,] board);

    virtual void Update_Position(Vector2 new_cell, Vector2 new_pos)
    {
        board_position = (Vector2I)new_cell;
        (this as Node2D).Position = new_pos;
    }

    string Notation();

    public bool Check_Special_Move(IPiece[,] board, Vector2I target);

    //for capture, target destination is null
    public (IPiece secondary_target, Vector2I sec_target_origin, Vector2I? sec_target_dest) Perform_Special_Move(IPiece[,] board, Vector2I target);

}

