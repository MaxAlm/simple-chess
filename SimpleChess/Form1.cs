using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleChess
{
    public partial class Form1 : Form
    {
        /////////////
        // METHODS //
        /////////////

        private void CreateChessboard()
        {
            // Create picture box template
            PictureBox pictureBox1 = new PictureBox() { Width = this.Width - 10, Height = this.Height - 30, Visible = false };
            this.Controls.Add(pictureBox1);

            // Create chessboard
            int color = 0;
            Size size = new Size(pictureBox1.Width / 8, pictureBox1.Height / 8);

            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                if (color == 1) { color = 0; }
                else { color++; }
                
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    tiles[i, j] = new PictureBox() { SizeMode = PictureBoxSizeMode.StretchImage, BackgroundImageLayout = ImageLayout.Stretch, BorderStyle = BorderStyle.None };
                    tiles[i, j].Left = i * size.Width;
                    tiles[i, j].Top = j * size.Height;
                    tiles[i, j].Width = size.Width;
                    tiles[i, j].Height = size.Height;
                    this.Controls.Add(tiles[i, j]);

                    // Set color
                    if (color == 0)
                    {
                        tiles[i, j].BackColor = Color.FromArgb(255, 240, 218, 181);
                    }
                    else
                    {
                        tiles[i, j].BackColor = Color.FromArgb(255, 181, 135, 99);
                    }

                    tiles[i, j].Tag = new TileInfo(i, j, (int)TYPE.NONE, (int)COLOR.NONE, false, false);
                    tiles[i, j].Click += new EventHandler(Click_Tile);

                    void Click_Tile(object sender, EventArgs e)
                    {
                        if (game_active)
                        {
                            var tag = (TileInfo)(sender as PictureBox).Tag;
                            var tag_old = (TileInfo)tiles[current_tile.X, current_tile.Y].Tag;

                            if (tag.COLOR == player)
                            {
                                if (tag.SELECTED)
                                {
                                    UpdateChessboard();
                                    tag.SELECTED = false;
                                    whitelist.Clear();
                                }
                                else
                                {
                                    UpdateChessboard();
                                    whitelist.Clear();

                                    // Select and unselect tiles
                                    tag_old.SELECTED = false;
                                    tag.SELECTED = true;

                                    // Update current_tile
                                    current_tile.X = tag.X;
                                    current_tile.Y = tag.Y;
                                    current_tile.TYPE = tag.TYPE;
                                    current_tile.COLOR = tag.COLOR;

                                    // Select tile
                                    tiles[tag.X, tag.Y].BorderStyle = BorderStyle.Fixed3D;

                                    WhitelistTiles();
                                }
                            }
                            else
                            {
                                for (int i = 0; i < whitelist.Count; i++)
                                {
                                    if (whitelist[i].X == tag.X && whitelist[i].Y == tag.Y)
                                    {
                                        UpdateChessboard();

                                        // Unselect current tile
                                        tag_old.SELECTED = false;

                                        // Set animation picture
                                        animation_picture.Width = tiles[tag_old.X, tag_old.Y].Width;
                                        animation_picture.Height = tiles[tag_old.X, tag_old.Y].Height;
                                        animation_picture.Left = tiles[tag_old.X, tag_old.Y].Left;
                                        animation_picture.Top = tiles[tag_old.X, tag_old.Y].Top;

                                        left_distance = (tiles[tag_old.X, tag_old.Y].Left - tiles[tag.X, tag.Y].Left) / 30;
                                        top_distance = (tiles[tag_old.X, tag_old.Y].Top - tiles[tag.X, tag.Y].Top) / 30;

                                        ChangeAnimationImage();

                                        // Animate tile
                                        game_active = false;
                                        animation_picture.BackColor = tiles[tag_old.X, tag_old.Y].BackColor;
                                        animation_picture.Visible = true;
                                        tiles[tag_old.X, tag_old.Y].BackgroundImage = null;
                                        timer.Start();

                                        // Check if tile is king
                                        if (tag.TYPE == (int)TYPE.KING)
                                        {
                                            string wintxt;
                                            DialogResult mb;

                                            // Transfer info to new tile
                                            tag.TYPE = tag_old.TYPE;
                                            tag.COLOR = tag_old.COLOR;
                                            tag_old.TYPE = (int)TYPE.NONE;
                                            tag_old.COLOR = (int)COLOR.NONE;
                                            tag_old.FIRST = false;
                                            tag.FIRST = false;

                                            if (player == (int)COLOR.WHITE) { wintxt = "White Wins!"; }
                                            else { wintxt = "Black Wins!"; }

                                            mb = MessageBox.Show(wintxt, "Simple Chess", MessageBoxButtons.OK);

                                            if (mb == DialogResult.OK)
                                            {
                                                NewGame();
                                                ChangeImages();
                                                break;
                                            }
                                        }

                                        // Transfer info to new tile
                                        tag.TYPE = tag_old.TYPE;
                                        tag.COLOR = tag_old.COLOR;
                                        tag_old.TYPE = (int)TYPE.NONE;
                                        tag_old.COLOR = (int)COLOR.NONE;
                                        tag_old.FIRST = false;
                                        tag.FIRST = false;

                                        whitelist.Clear();

                                        // Change player
                                        if (player == (int)COLOR.WHITE)
                                        {
                                            player = (int)COLOR.BLACK;
                                        }
                                        else
                                        {
                                            player = (int)COLOR.WHITE;
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    // Change color variable
                    if (color == 1) { color = 0; }
                    else { color++; }
                }
            }

            this.Height += 39;
        }

        private void NewGame()
        {
            // Set starting player
            player = (int)COLOR.WHITE;

            // Clear whitelist
            whitelist.Clear();

            // Reset animation settings
            timer.Stop();
            animation_picture.Visible = false;
            animtaion_tick = 0;
            game_active = true;
            
            // Reset tile info
            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    var tag0 = (TileInfo)tiles[i, j].Tag;

                    tag0.TYPE = (int)TYPE.NONE;
                    tag0.COLOR = (int)COLOR.NONE;
                    tag0.FIRST = false;
                    tag0.SELECTED = false;
                }
            }
            
            /////////////////
            // PLACE PAWNS //
            /////////////////

            // Black pawns
            for (int i = 0; i < 8; i++)
            {
                var tag0 = (TileInfo)tiles[i, 1].Tag;

                tag0.TYPE = (int)TYPE.PAWN;
                tag0.COLOR = (int)COLOR.BLACK;
                tag0.FIRST = true;
            }

            // White pawns
            for (int i = 0; i < 8; i++)
            {
                var tag0 = (TileInfo)tiles[i, 6].Tag;

                tag0.TYPE = (int)TYPE.PAWN;
                tag0.COLOR = (int)COLOR.WHITE;
                tag0.FIRST = true;
            }

            /////////////////
            // PLACE ROOKS //
            /////////////////

            // Black rooks
            var tag = (TileInfo)tiles[0, 0].Tag;

            tag.TYPE = (int)TYPE.ROOK;
            tag.COLOR = (int)COLOR.BLACK;

            tag = (TileInfo)tiles[7, 0].Tag;

            tag.TYPE = (int)TYPE.ROOK;
            tag.COLOR = (int)COLOR.BLACK;

            // White rooks
            tag = (TileInfo)tiles[0, 7].Tag;

            tag.TYPE = (int)TYPE.ROOK;
            tag.COLOR = (int)COLOR.WHITE;

            tag = (TileInfo)tiles[7, 7].Tag;

            tag.TYPE = (int)TYPE.ROOK;
            tag.COLOR = (int)COLOR.WHITE;

            ///////////////////
            // PLACE KNIGHTS //
            ///////////////////

            // Black knights
            tag = (TileInfo)tiles[1, 0].Tag;

            tag.TYPE = (int)TYPE.KNIGHT;
            tag.COLOR = (int)COLOR.BLACK;

            tag = (TileInfo)tiles[6, 0].Tag;

            tag.TYPE = (int)TYPE.KNIGHT;
            tag.COLOR = (int)COLOR.BLACK;

            // White knights
            tag = (TileInfo)tiles[1, 7].Tag;

            tag.TYPE = (int)TYPE.KNIGHT;
            tag.COLOR = (int)COLOR.WHITE;

            tag = (TileInfo)tiles[6, 7].Tag;

            tag.TYPE = (int)TYPE.KNIGHT;
            tag.COLOR = (int)COLOR.WHITE;

            ///////////////////
            // PLACE BISHOPS //
            ///////////////////

            // Black bishops
            tag = (TileInfo)tiles[2, 0].Tag;

            tag.TYPE = (int)TYPE.BISHOP;
            tag.COLOR = (int)COLOR.BLACK;

            tag = (TileInfo)tiles[5, 0].Tag;

            tag.TYPE = (int)TYPE.BISHOP;
            tag.COLOR = (int)COLOR.BLACK;

            // White bishops
            tag = (TileInfo)tiles[2, 7].Tag;

            tag.TYPE = (int)TYPE.BISHOP;
            tag.COLOR = (int)COLOR.WHITE;

            tag = (TileInfo)tiles[5, 7].Tag;

            tag.TYPE = (int)TYPE.BISHOP;
            tag.COLOR = (int)COLOR.WHITE;

            //////////////////
            // PLACE QUEENS //
            //////////////////

            // Black queen
            tag = (TileInfo)tiles[3, 0].Tag;

            tag.TYPE = (int)TYPE.QUEEN;
            tag.COLOR = (int)COLOR.BLACK;

            // White queen
            tag = (TileInfo)tiles[3, 7].Tag;

            tag.TYPE = (int)TYPE.QUEEN;
            tag.COLOR = (int)COLOR.WHITE;

            /////////////////
            // PLACE KINGS //
            /////////////////

            // Black king
            tag = (TileInfo)tiles[4, 0].Tag;

            tag.TYPE = (int)TYPE.KING;
            tag.COLOR = (int)COLOR.BLACK;

            // White king
            tag = (TileInfo)tiles[4, 7].Tag;

            tag.TYPE = (int)TYPE.KING;
            tag.COLOR = (int)COLOR.WHITE;
        }

        private void ChangeImages()
        {
            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    var tag = (TileInfo)tiles[i, j].Tag;
                    
                    if (tag.TYPE == (int)TYPE.NONE)
                    {
                        tiles[tag.X, tag.Y].BackgroundImage = null;
                    }
                    else
                    {
                        switch (tag.TYPE)
                        {
                            case (int)TYPE.PAWN:
                                if (tag.COLOR == (int)COLOR.BLACK)
                                {
                                    tiles[tag.X, tag.Y].BackgroundImage = Image.FromFile(@"pawn_black.png");
                                }
                                else
                                {
                                    tiles[tag.X, tag.Y].BackgroundImage = Image.FromFile(@"pawn_white.png");
                                }
                                break;

                            case (int)TYPE.ROOK:
                                if (tag.COLOR == (int)COLOR.BLACK)
                                {
                                    tiles[tag.X, tag.Y].BackgroundImage = Image.FromFile(@"rook_black.png");
                                }
                                else
                                {
                                    tiles[tag.X, tag.Y].BackgroundImage = Image.FromFile(@"rook_white.png");
                                }
                                break;

                            case (int)TYPE.KNIGHT:
                                if (tag.COLOR == (int)COLOR.BLACK)
                                {
                                    tiles[tag.X, tag.Y].BackgroundImage = Image.FromFile(@"knight_black.png");
                                }
                                else
                                {
                                    tiles[tag.X, tag.Y].BackgroundImage = Image.FromFile(@"knight_white.png");
                                }
                                break;

                            case (int)TYPE.BISHOP:
                                if (tag.COLOR == (int)COLOR.BLACK)
                                {
                                    tiles[tag.X, tag.Y].BackgroundImage = Image.FromFile(@"bishop_black.png");
                                }
                                else
                                {
                                    tiles[tag.X, tag.Y].BackgroundImage = Image.FromFile(@"bishop_white.png");
                                }
                                break;

                            case (int)TYPE.QUEEN:
                                if (tag.COLOR == (int)COLOR.BLACK)
                                {
                                    tiles[tag.X, tag.Y].BackgroundImage = Image.FromFile(@"queen_black.png");
                                }
                                else
                                {
                                    tiles[tag.X, tag.Y].BackgroundImage = Image.FromFile(@"queen_white.png");
                                }
                                break;

                            case (int)TYPE.KING:
                                if (tag.COLOR == (int)COLOR.BLACK)
                                {
                                    tiles[tag.X, tag.Y].BackgroundImage = Image.FromFile(@"king_black.png");
                                }
                                else
                                {
                                    tiles[tag.X, tag.Y].BackgroundImage = Image.FromFile(@"king_white.png");
                                }
                                break;
                        }
                    }
                }
            }
        }

        private void ChangeAnimationImage()
        {
            var tag = (TileInfo)tiles[current_tile.X, current_tile.Y].Tag;

            switch (tag.TYPE)
            {
                case (int)TYPE.PAWN:
                    if (tag.COLOR == (int)COLOR.BLACK)
                    {
                        animation_picture.BackgroundImage = Image.FromFile(@"pawn_black.png");
                    }
                    else
                    {
                        animation_picture.BackgroundImage = Image.FromFile(@"pawn_white.png");
                    }
                    break;

                case (int)TYPE.ROOK:
                    if (tag.COLOR == (int)COLOR.BLACK)
                    {
                        animation_picture.BackgroundImage = Image.FromFile(@"rook_black.png");
                    }
                    else
                    {
                        animation_picture.BackgroundImage = Image.FromFile(@"rook_white.png");
                    }
                    break;

                case (int)TYPE.KNIGHT:
                    if (tag.COLOR == (int)COLOR.BLACK)
                    {
                        animation_picture.BackgroundImage = Image.FromFile(@"knight_black.png");
                    }
                    else
                    {
                        animation_picture.BackgroundImage = Image.FromFile(@"knight_white.png");
                    }
                    break;

                case (int)TYPE.BISHOP:
                    if (tag.COLOR == (int)COLOR.BLACK)
                    {
                        animation_picture.BackgroundImage = Image.FromFile(@"bishop_black.png");
                    }
                    else
                    {
                        animation_picture.BackgroundImage = Image.FromFile(@"bishop_white.png");
                    }
                    break;

                case (int)TYPE.QUEEN:
                    if (tag.COLOR == (int)COLOR.BLACK)
                    {
                        animation_picture.BackgroundImage = Image.FromFile(@"queen_black.png");
                    }
                    else
                    {
                        animation_picture.BackgroundImage = Image.FromFile(@"queen_white.png");
                    }
                    break;

                case (int)TYPE.KING:
                    if (tag.COLOR == (int)COLOR.BLACK)
                    {
                        animation_picture.BackgroundImage = Image.FromFile(@"king_black.png");
                    }
                    else
                    {
                        animation_picture.BackgroundImage = Image.FromFile(@"king_white.png");
                    }
                    break;
            }
        }

        private void UpdateChessboard()
        {
            for (int i = 0; i < tiles.GetLength(0); i++)
            {
                for (int j = 0; j < tiles.GetLength(1); j++)
                {
                    var tag = (TileInfo)tiles[i, j].Tag;

                    tiles[tag.X, tag.Y].Image = null;
                    tiles[tag.X, tag.Y].BorderStyle = BorderStyle.None;
                }
            }
        }

        private void WhitelistTiles()
        {
            var tag = (TileInfo)tiles[current_tile.X, current_tile.Y].Tag;

            switch (tag.TYPE)
            {
                case (int)TYPE.PAWN:

                    // Check standard pawn tiles
                    int num = 0;

                    while (true)
                    {
                        // Checks the tile above/below the pawn
                        if (player == (int)COLOR.WHITE) { num--; }
                        else { num++; }

                        var tag0 = (TileInfo)tiles[tag.X, tag.Y + num].Tag;

                        if (tag0.TYPE == (int)TYPE.NONE)
                        {
                            tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                            whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                        }
                        else
                        {
                            break;
                        }

                        if (tag.FIRST)
                        {
                            if (num == 2 || num == -2)
                                break;
                        }
                        else
                        {
                            break;
                        }
                    }

                    // Check if opponent is near
                    if (player == (int)COLOR.WHITE)
                    {
                        try
                        {
                            var tag0 = (TileInfo)tiles[tag.X + 1, tag.Y - 1].Tag;

                            if (tag0.COLOR == (int)COLOR.BLACK)
                            {
                                tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                                whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                            }
                        } catch { }

                        try
                        {
                            var tag1 = (TileInfo)tiles[tag.X - 1, tag.Y - 1].Tag;

                            if (tag1.COLOR == (int)COLOR.BLACK)
                            {
                                tiles[tag1.X, tag1.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                                whitelist.Add(new Whitelist(tag1.X, tag1.Y));
                            }
                        } catch { }
                    }
                    else // if player is black
                    {
                        try
                        {
                            var tag0 = (TileInfo)tiles[tag.X + 1, tag.Y + 1].Tag;

                            if (tag0.COLOR == (int)COLOR.WHITE)
                            {
                                tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                                whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                            }
                        }
                        catch { }

                        try
                        {
                            var tag1 = (TileInfo)tiles[tag.X - 1, tag.Y + 1].Tag;

                            if (tag1.COLOR == (int)COLOR.WHITE)
                            {
                                tiles[tag1.X, tag1.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                                whitelist.Add(new Whitelist(tag1.X, tag1.Y));
                            }
                        }
                        catch { }
                    }
                    break;

                case (int)TYPE.ROOK:
                    // Check up
                    try
                    {
                        num = 1;

                        while (true)
                        {
                            var tag0 = (TileInfo)tiles[tag.X, tag.Y - num].Tag;

                            if (tag0.TYPE == (int)TYPE.NONE)
                            {
                                tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                                whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                            }
                            else if (tag0.COLOR != player)
                            {
                                tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                                whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                                break;
                            }
                            else
                            {
                                break;
                            }

                            num++;
                        }
                    } catch { }

                    // Check down
                    try
                    {
                        num = 1;

                        while (true)
                        {
                            var tag0 = (TileInfo)tiles[tag.X, tag.Y + num].Tag;

                            if (tag0.TYPE == (int)TYPE.NONE)
                            {
                                tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                                whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                            }
                            else if (tag0.COLOR != player)
                            {
                                tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                                whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                                break;
                            }
                            else
                            {
                                break;
                            }

                            num++;
                        }
                    }
                    catch { }

                    // Check left
                    try
                    {
                        num = 1;

                        while (true)
                        {
                            var tag0 = (TileInfo)tiles[tag.X - num, tag.Y].Tag;

                            if (tag0.TYPE == (int)TYPE.NONE)
                            {
                                tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                                whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                            }
                            else if (tag0.COLOR != player)
                            {
                                tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                                whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                                break;
                            }
                            else
                            {
                                break;
                            }

                            num++;
                        }
                    }
                    catch { }

                    // Check right
                    try
                    {
                        num = 1;

                        while (true)
                        {
                            var tag0 = (TileInfo)tiles[tag.X + num, tag.Y].Tag;

                            if (tag0.TYPE == (int)TYPE.NONE)
                            {
                                tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                                whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                            }
                            else if (tag0.COLOR != player)
                            {
                                tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                                whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                                break;
                            }
                            else
                            {
                                break;
                            }

                            num++;
                        }
                    }
                    catch { }
                    break;

                case (int)TYPE.KNIGHT:
                    // Check up (top left)
                    try
                    {
                        var tag0 = (TileInfo)tiles[tag.X - 1, tag.Y - 2].Tag;

                        if (tag0.TYPE == (int)TYPE.NONE)
                        {
                            tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                            whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                        }
                        else if (tag0.COLOR != player)
                        {
                            tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                            whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                        }
                    }
                    catch { }

                    // Check up (bottom left)
                    try
                    {
                        var tag0 = (TileInfo)tiles[tag.X - 2, tag.Y - 1].Tag;

                        if (tag0.TYPE == (int)TYPE.NONE)
                        {
                            tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                            whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                        }
                        else if (tag0.COLOR != player)
                        {
                            tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                            whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                        }
                    }
                    catch { }

                    // Check up (top right)
                    try
                    {
                        var tag0 = (TileInfo)tiles[tag.X + 1, tag.Y - 2].Tag;

                        if (tag0.TYPE == (int)TYPE.NONE)
                        {
                            tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                            whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                        }
                        else if (tag0.COLOR != player)
                        {
                            tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                            whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                        }
                    }
                    catch { }

                    // Check up (bottom right)
                    try
                    {
                        var tag0 = (TileInfo)tiles[tag.X + 2, tag.Y - 1].Tag;

                        if (tag0.TYPE == (int)TYPE.NONE)
                        {
                            tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                            whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                        }
                        else if (tag0.COLOR != player)
                        {
                            tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                            whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                        }
                    }
                    catch { }

                    // Check down (bottom left)
                    try
                    {
                        var tag0 = (TileInfo)tiles[tag.X - 1, tag.Y + 2].Tag;

                        if (tag0.TYPE == (int)TYPE.NONE)
                        {
                            tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                            whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                        }
                        else if (tag0.COLOR != player)
                        {
                            tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                            whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                        }
                    }
                    catch { }

                    // Check down (top left)
                    try
                    {
                        var tag0 = (TileInfo)tiles[tag.X - 2, tag.Y + 1].Tag;

                        if (tag0.TYPE == (int)TYPE.NONE)
                        {
                            tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                            whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                        }
                        else if (tag0.COLOR != player)
                        {
                            tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                            whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                        }
                    }
                    catch { }

                    // Check down (bottom right)
                    try
                    {
                        var tag0 = (TileInfo)tiles[tag.X + 1, tag.Y + 2].Tag;

                        if (tag0.TYPE == (int)TYPE.NONE)
                        {
                            tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                            whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                        }
                        else if (tag0.COLOR != player)
                        {
                            tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                            whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                        }
                    }
                    catch { }

                    // Check down (top right)
                    try
                    {
                        var tag0 = (TileInfo)tiles[tag.X + 2, tag.Y + 1].Tag;

                        if (tag0.TYPE == (int)TYPE.NONE)
                        {
                            tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                            whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                        }
                        else if (tag0.COLOR != player)
                        {
                            tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                            whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                        }
                    }
                    catch { }
                    break;

                case (int)TYPE.BISHOP:
                    // Check left up
                    try
                    {
                        num = 1;

                        while (true)
                        {
                            var tag0 = (TileInfo)tiles[tag.X - num, tag.Y - num].Tag;

                            if (tag0.TYPE == (int)TYPE.NONE)
                            {
                                tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                                whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                            }
                            else if (tag0.COLOR != player)
                            {
                                tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                                whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                                break;
                            }
                            else
                            {
                                break;
                            }

                            num++;
                        }
                    }
                    catch { }

                    // Check right up
                    try
                    {
                        num = 1;

                        while (true)
                        {
                            var tag0 = (TileInfo)tiles[tag.X + num, tag.Y - num].Tag;

                            if (tag0.TYPE == (int)TYPE.NONE)
                            {
                                tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                                whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                            }
                            else if (tag0.COLOR != player)
                            {
                                tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                                whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                                break;
                            }
                            else
                            {
                                break;
                            }

                            num++;
                        }
                    }
                    catch { }

                    // Check left down
                    try
                    {
                        num = 1;

                        while (true)
                        {
                            var tag0 = (TileInfo)tiles[tag.X - num, tag.Y + num].Tag;

                            if (tag0.TYPE == (int)TYPE.NONE)
                            {
                                tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                                whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                            }
                            else if (tag0.COLOR != player)
                            {
                                tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                                whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                                break;
                            }
                            else
                            {
                                break;
                            }

                            num++;
                        }
                    }
                    catch { }

                    // Check right down
                    try
                    {
                        num = 1;

                        while (true)
                        {
                            var tag0 = (TileInfo)tiles[tag.X + num, tag.Y + num].Tag;

                            if (tag0.TYPE == (int)TYPE.NONE)
                            {
                                tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                                whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                            }
                            else if (tag0.COLOR != player)
                            {
                                tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                                whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                                break;
                            }
                            else
                            {
                                break;
                            }

                            num++;
                        }
                    }
                    catch { }
                    break;

                case (int)TYPE.QUEEN:
                    // Check up
                    try
                    {
                        num = 1;

                        while (true)
                        {
                            var tag0 = (TileInfo)tiles[tag.X, tag.Y - num].Tag;

                            if (tag0.TYPE == (int)TYPE.NONE)
                            {
                                tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                                whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                            }
                            else if (tag0.COLOR != player)
                            {
                                tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                                whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                                break;
                            }
                            else
                            {
                                break;
                            }

                            num++;
                        }
                    }
                    catch { }

                    // Check down
                    try
                    {
                        num = 1;

                        while (true)
                        {
                            var tag0 = (TileInfo)tiles[tag.X, tag.Y + num].Tag;

                            if (tag0.TYPE == (int)TYPE.NONE)
                            {
                                tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                                whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                            }
                            else if (tag0.COLOR != player)
                            {
                                tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                                whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                                break;
                            }
                            else
                            {
                                break;
                            }

                            num++;
                        }
                    }
                    catch { }

                    // Check left
                    try
                    {
                        num = 1;

                        while (true)
                        {
                            var tag0 = (TileInfo)tiles[tag.X - num, tag.Y].Tag;

                            if (tag0.TYPE == (int)TYPE.NONE)
                            {
                                tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                                whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                            }
                            else if (tag0.COLOR != player)
                            {
                                tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                                whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                                break;
                            }
                            else
                            {
                                break;
                            }

                            num++;
                        }
                    }
                    catch { }

                    // Check right
                    try
                    {
                        num = 1;

                        while (true)
                        {
                            var tag0 = (TileInfo)tiles[tag.X + num, tag.Y].Tag;

                            if (tag0.TYPE == (int)TYPE.NONE)
                            {
                                tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                                whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                            }
                            else if (tag0.COLOR != player)
                            {
                                tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                                whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                                break;
                            }
                            else
                            {
                                break;
                            }

                            num++;
                        }
                    }
                    catch { }

                    // Check left up
                    try
                    {
                        num = 1;

                        while (true)
                        {
                            var tag0 = (TileInfo)tiles[tag.X - num, tag.Y - num].Tag;

                            if (tag0.TYPE == (int)TYPE.NONE)
                            {
                                tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                                whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                            }
                            else if (tag0.COLOR != player)
                            {
                                tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                                whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                                break;
                            }
                            else
                            {
                                break;
                            }

                            num++;
                        }
                    }
                    catch { }

                    // Check right up
                    try
                    {
                        num = 1;

                        while (true)
                        {
                            var tag0 = (TileInfo)tiles[tag.X + num, tag.Y - num].Tag;

                            if (tag0.TYPE == (int)TYPE.NONE)
                            {
                                tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                                whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                            }
                            else if (tag0.COLOR != player)
                            {
                                tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                                whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                                break;
                            }
                            else
                            {
                                break;
                            }

                            num++;
                        }
                    }
                    catch { }

                    // Check left down
                    try
                    {
                        num = 1;

                        while (true)
                        {
                            var tag0 = (TileInfo)tiles[tag.X - num, tag.Y + num].Tag;

                            if (tag0.TYPE == (int)TYPE.NONE)
                            {
                                tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                                whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                            }
                            else if (tag0.COLOR != player)
                            {
                                tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                                whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                                break;
                            }
                            else
                            {
                                break;
                            }

                            num++;
                        }
                    }
                    catch { }

                    // Check right down
                    try
                    {
                        num = 1;

                        while (true)
                        {
                            var tag0 = (TileInfo)tiles[tag.X + num, tag.Y + num].Tag;

                            if (tag0.TYPE == (int)TYPE.NONE)
                            {
                                tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                                whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                            }
                            else if (tag0.COLOR != player)
                            {
                                tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                                whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                                break;
                            }
                            else
                            {
                                break;
                            }

                            num++;
                        }
                    }
                    catch { }
                    break;

                case (int)TYPE.KING:
                    // Check around
                    for (int i = 1; i >= -1; i--)
                    {
                        for (int j = 1; j >= -1; j--)
                        {
                            try
                            {
                                var tag0 = (TileInfo)tiles[tag.X - j, tag.Y - i].Tag;

                                if (tag0.TYPE == (int)TYPE.NONE)
                                {
                                    tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                                    whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                                }
                                else if (tag0.COLOR != player)
                                {
                                    tiles[tag0.X, tag0.Y].Image = Image.FromFile(@"chess_highlight_black.png");
                                    whitelist.Add(new Whitelist(tag0.X, tag0.Y));
                                }
                            }
                            catch { }
                        }
                    }
                    break;
            }
        }

        ///////////////
        // VARIABLES //
        ///////////////

        PictureBox[,] tiles;        // Chessboard
        bool game_active = true;    // Controls if game is active or not
        Timer timer = new Timer();  // Animation timer
        int animtaion_tick = 0;     // Controls animation ticks
        int left_distance;          // Sets the left distance to move the tile every tick
        int top_distance;           // Sets the top distance to move the tile every tick
        int player;                 // Keeps track of who's turn it is
        TileInfo current_tile;      // Stores info from the currently selected tile
        List<Whitelist> whitelist;  // Stores all withelisted tiles

        enum TYPE { NONE = 1, PAWN = 2, KNIGHT = 3, BISHOP = 4, ROOK = 5, QUEEN = 6, KING = 7 }
        enum COLOR { NONE = 0, BLACK = 1, WHITE = 2 }

        ////////////////
        // CODE START //
        ////////////////

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Create 2D picture box array
            tiles = new PictureBox[8, 8];

            // Set variables
            current_tile = new TileInfo(0, 0, (int)TYPE.NONE, (int)COLOR.NONE, false, false);
            whitelist = new List<Whitelist>();

            // Set animation timer
            timer.Interval = 1;
            timer.Tick += timer_tick;

            CreateChessboard();
            NewGame();
            ChangeImages();
            UpdateChessboard();
        }

        private void timer_tick(object sender, EventArgs e)
        {
            if (animtaion_tick == 30)
            {
                timer.Stop();
                animation_picture.Visible = false;
                animtaion_tick = 0;
                ChangeImages();
                game_active = true;
            }
            else
            {
                animation_picture.Top -= top_distance;
                animation_picture.Left -= left_distance;

                animtaion_tick++;
            }
        }
    }
}
