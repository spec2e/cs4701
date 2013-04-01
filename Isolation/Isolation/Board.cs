﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Isolation
{
    public class Board : IEquatable<Board>
    {
        public Board(string flatBoard, Player myPlayer) : this()
        {
            if (flatBoard.Length != 64)
            {
                throw new ArgumentException("Invalid board.");
            }

            Initialize(myPlayer);

            for (byte i = 0; i < 8; i++)
            {
                for (byte j = 0; j < 8; j++)
                {
                    var space = GetSpaceFromChar(flatBoard[i * 7 + j]);
                    _board[i, j] = space;

                    if (space == BoardSpaceValue.PlayerX)
                    {
                        Xposition = new BoardSpace(i, j);
                    }
                    else if (space == BoardSpaceValue.PlayerO)
                    {
                        Oposition = new BoardSpace(i, j);
                    }
                    else if (space == BoardSpaceValue.Filled)
                    {
                        EmptySpacesRemaining--;
                    }
                }
            }

            PlayerToMove = EmptySpacesRemaining % 2 == 0 ? Player.X : Player.O;
        }

        private readonly BoardSpaceValue[,] _board;

        public int EmptySpacesRemaining { get; private set; }
        public Player PlayerToMove { get; private set; }
        public Player MyPlayer { get; private set; }
        public Player OpponentPlayer { get; private set; }
        public BoardSpace Xposition { get; private set; }
        public BoardSpace Oposition { get; private set; }

        // override of operator[]
        public BoardSpaceValue this[byte row, byte col]
        {
            get { return _board[row, col]; }
        }

        public void Initialize(Player myPlayer)
        {
            MyPlayer = myPlayer;
            OpponentPlayer = MyPlayer == Player.X ? Player.O : Player.X;
        }
        
        private Board()
        {
            _board = new BoardSpaceValue[8, 8];
            EmptySpacesRemaining = 62;
        }

        public static Board ConstructInitialBoard(Player myPlayer)
        {
            var board = new Board
            {
                Xposition = new BoardSpace(0, 0),
                Oposition = new BoardSpace(7, 7),
                PlayerToMove = Player.X,
            };

            board.Initialize(myPlayer);

            for (byte i = 0; i < 8; i++)
            {
                for (byte j = 0; j < 8; j++)
                {
                    board._board[i, j] = BoardSpaceValue.Empty;
                }
            }

            board._board[0, 0] = BoardSpaceValue.PlayerX;
            board._board[7, 7] = BoardSpaceValue.PlayerO;

            return board;
        }

        public Board Copy()
        {
            var board = new Board
            {
                Xposition = Xposition,
                Oposition = Oposition,
                PlayerToMove = PlayerToMove,
                EmptySpacesRemaining = EmptySpacesRemaining,
            };

            board.Initialize(MyPlayer);

            for (byte i = 0; i < 8; i++)
            {
                for (byte j = 0; j < 8; j++)
                {
                    board._board[i, j] = _board[i, j];
                }
            }

            return board;
        }

        #region perform move

        public void Move(BoardSpace move)
        {
            if (PlayerToMove == Player.X)
            {
                _board[Xposition.Row, Xposition.Col] = BoardSpaceValue.Filled;
                _board[move.Row, move.Col] = BoardSpaceValue.PlayerX;
                Xposition = move;
                PlayerToMove = Player.O;
            }
            else
            {
                _board[Oposition.Row, Oposition.Col] = BoardSpaceValue.Filled;
                _board[move.Row, move.Col] = BoardSpaceValue.PlayerO;
                Oposition = move;
                PlayerToMove = Player.X;
            }
            EmptySpacesRemaining--;
        }

        #endregion

        #region MoveGenerator

        public List<BoardSpace> GetValidMoves()
        {
            return PlayerToMove == Player.X ? GetMoves(Xposition) : GetMoves(Oposition);
        } 

        private List<BoardSpace> GetMoves(BoardSpace currentPosition)
        {
            var moves = new List<BoardSpace>();

            #region vertical moves

            // walk down from currentPosition
            for (var i = currentPosition.Row + 1; i < 8; i++)
            {
                if (_board[i, currentPosition.Col] == BoardSpaceValue.Empty)
                {
                    moves.Add(new BoardSpace((byte)i, currentPosition.Col));
                }
                else { break; }
            }

            // walk up from currentPosition
            for (var i = currentPosition.Row - 1; i >= 0; i--)
            {
                if (_board[i, currentPosition.Col] == BoardSpaceValue.Empty)
                {
                    moves.Add(new BoardSpace((byte)i, currentPosition.Col));
                }
                else { break; }
            }

            #endregion

            #region horizontal moves

            // walk right from currentPosition
            for (var j = currentPosition.Col + 1; j < 8; j++)
            {
                if (_board[currentPosition.Row, j] == BoardSpaceValue.Empty)
                {
                    moves.Add(new BoardSpace(currentPosition.Row, (byte)j));
                }
                else { break; }
            }

            // walk left from currentPosition
            for (var j = currentPosition.Col - 1; j >= 0; j--)
            {
                if (_board[currentPosition.Row, j] == BoardSpaceValue.Empty)
                {
                    moves.Add(new BoardSpace(currentPosition.Row, (byte)j));
                }
                else { break; }
            }

            #endregion

            #region diagonal moves

            // walk down-right from currentPosition
            for (int i = currentPosition.Row + 1, j = currentPosition.Col + 1; i < 8 && j < 8; i++, j++)
            {
                if (_board[i, j] == BoardSpaceValue.Empty)
                {
                    moves.Add(new BoardSpace((byte)i, (byte)j));
                }
                else { break; }
            }

            // walk down-left from currentPosition
            for (int i = currentPosition.Row + 1, j = currentPosition.Col - 1; i < 8 && j >=0; i++, j--)
            {
                if (_board[i, j] == BoardSpaceValue.Empty)
                {
                    moves.Add(new BoardSpace((byte)i, (byte)j));
                }
                else { break; }
            }

            // walk up-right from currentPosition
            for (int i = currentPosition.Row - 1, j = currentPosition.Col + 1; i >=0 && j < 8; i--, j++)
            {
                if (_board[i, j] == BoardSpaceValue.Empty)
                {
                    moves.Add(new BoardSpace((byte)i, (byte)j));
                }
                else { break; }
            }

            // walk up-left from currentPosition
            for (int i = currentPosition.Row - 1, j = currentPosition.Col - 1; i >=0 && j >=0; i--, j--)
            {
                if (_board[i, j] == BoardSpaceValue.Empty)
                {
                    moves.Add(new BoardSpace((byte)i, (byte)j));
                }
                else { break; }
            }

            #endregion

            return moves;
        } 

        public List<BoardSpace> GetMyValidMoves()
        {
            return MyPlayer == Player.X ? GetMoves(Xposition) : GetMoves(Oposition);
        }

        public List<BoardSpace> GetOpponentValidMoves()
        {
            return MyPlayer == Player.X ? GetMoves(Oposition) : GetMoves(Xposition);
        }

        public bool IsValidMove(BoardSpace move)
        {
            return GetValidMoves().Any(x => x.Equals(move));
        }

        #endregion

        #region equality and hashing

        public bool Equals(Board other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            for (byte i = 0; i < 8; i++)
            {
                for (byte j = 0; j < 8; j++)
                {
                    if (_board[i, j] != other._board[i, j])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Board);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var sum = 0;
                for (byte i = 0; i < 8; i++)
                {
                    for (byte j = 0; j < 8; j++)
                    {
                        var positionalValue = (i * 8 + (j + 1)) * ((byte)_board[i, j]);
                        sum += 17 ^ 23 * positionalValue.GetHashCode();
                    }
                }
                return sum;
            }
        }

        #endregion

        #region printing

        private static char GetCharFromSpace(BoardSpaceValue spaceValue)
        {
            switch (spaceValue)
            {
                case BoardSpaceValue.Empty:
                    return '-';
                case BoardSpaceValue.Filled:
                    return '*';
                case BoardSpaceValue.PlayerO:
                    return 'o';
                case BoardSpaceValue.PlayerX:
                    return 'x';
                default:
                    throw new Exception("Unhandled board space.");
            }
        }

        private static BoardSpaceValue GetSpaceFromChar(char c)
        {
            switch (c)
            {
                case '*':
                    return BoardSpaceValue.Filled;
                case '-':
                    return BoardSpaceValue.Empty;
                case 'o':
                case 'O':
                    return BoardSpaceValue.PlayerO;
                case 'x':
                case 'X':
                    return BoardSpaceValue.PlayerX;
                default:
                    throw new Exception("Invalid board space.");
            }
        }

        public string ToFlatString()
        {
            var builder = new StringBuilder();
            for (var i = 0; i < 8; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    builder.Append(GetCharFromSpace(_board[i, j]));
                }
            }
            return builder.ToString();
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine("  1 2 3 4 5 6 7 8");
            for (var i = 0; i < 8; i++)
            {
                builder.Append(i + 1).Append(" ");
                for (var j = 0; j < 8; j++)
                {
                    builder.Append(GetCharFromSpace(_board[i, j])).Append(" ");
                }

                if (i != 7)
                {
                    builder.AppendLine();
                }
            }
            return builder.ToString();
        }

        #endregion
    }
}