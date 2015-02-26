
public enum Command {
	Register,
	PuzzleFinished,
    GameStart,
	Ready,
	Pause,
	QrCodeSend,
	GetGameState,
    GameStateResponse,
    GetImage,
    GetImageResponse,
	PieceScanned,
	PenaltyTimeAdd,
	AreYouThere,
    NoCommand,
    MalformedCommand,
    Registered,
    CreatePuzzle,
    CreatePuzzlePart,
    GameFinished,
    CheckPuzzleFinished,
    RegisterGameStatusListener,
    GameStatusChanged
}

static class CommandMethods
{
    public static string getString(this Command s1)
    {
        switch (s1)
        {
            case Command.Register:
                return "REGISTER";
            case Command.Pause:
                return "GAME_PAUSED";
            case Command.Ready:
                return "READY";
            case Command.GetGameState:
                return "GET_GAME_INFO";
            case Command.GameStateResponse:
                return "GAME_INFO";
            case Command.MalformedCommand:
                return "MALFORMED_COMMAND";
            case Command.GetImage:
                return "GET_PUZZLE_PART";
            case Command.GetImageResponse:
                return "PUZZLE_PART";
            case Command.Registered:
                return "REGISTERED";
            case Command.QrCodeSend:
                return "SHOW_QR";
            case Command.CreatePuzzle:
                return "CREATE_PUZZLE";
            case Command.CreatePuzzlePart:
                return "CREATE_PUZZLE_PART";
            case Command.GameStart:
                return "GAME_START";
            case Command.PieceScanned:
                return "PART_UNLOCKED";
            case Command.PuzzleFinished:
                return "PUZZLE_FINISHED";
            case Command.GameFinished:
                return "GAME_FINISHED";
            case Command.CheckPuzzleFinished:
                return "CHECK_PUZZLE_FINISHED";
            case Command.RegisterGameStatusListener:
                return "REGISTER_GAME_STATUS_LISTENER";
            case Command.GameStatusChanged:
                return "GAME_STATUS_CHANGED";
            default:
                return "UNKNOWN COMMAND";
        }
    }

    public static Command getCommand(string commandString)
    {
        if (commandString.Equals("REGISTER"))
        {
            return Command.Register;
        }
        else if (commandString.Equals("GAME_PAUSED"))
        {
            return Command.Pause;
        } else if(commandString.Equals("READY")) 
        {
            return Command.Ready;
        }
        else if (commandString.Equals("GET_GAME_INFO"))
        {
            return Command.GetGameState;
        } 
        else if(commandString.Equals("GAME_INFO")) 
        {
            return Command.GameStateResponse;
        }
        else if (commandString.Equals("MALFORMED_COMMAND"))
        {
            return Command.MalformedCommand;
        }
        else if (commandString.Equals("REGISTERED"))
        {
            return Command.Registered;
        }
        else if (commandString.Equals("SHOW_QR"))
        {
            return Command.QrCodeSend;
        } 
        else if(commandString.Equals("GET_PUZZLE_PART")) {
            return Command.GetImage;
        }
        else if (commandString.Equals("PUZZLE_PART"))
        {
            return Command.GetImageResponse;
        }
        else if (commandString.Equals("CREATE_PUZZLE"))
        {
            return Command.CreatePuzzle;
        }
        else if (commandString.Equals("CREATE_PUZZLE_PART"))
        {
            return Command.CreatePuzzlePart;
        }
        else if (commandString.Equals("GAME_START"))
        {
            return Command.GameStart;
        }
        else if (commandString.Equals("PART_UNLOCKED"))
        {
            return Command.PieceScanned;
        }
        else if (commandString.Equals("PUZZLE_FINISHED"))
        {
            return Command.PuzzleFinished;
        }
        else if (commandString.Equals("GAME_FINISHED"))
        {
            return Command.GameFinished;
        }
        else if (commandString.Equals("CHECK_PUZZLE_FINISHED"))
        {
            return Command.CheckPuzzleFinished;
        }
        else if (commandString.Equals("REGISTER_GAME_STATUS_LISTENER"))
        {
            return Command.RegisterGameStatusListener;
        }
        else if (commandString.Equals("GAME_STATUS_CHANGED"))
        {
            return Command.GameStatusChanged;
        }
        else
        {
            return Command.NoCommand;
        }
    }
}