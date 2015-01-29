
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
    GameFinished
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
                return "PAUSE";
            case Command.Ready:
                return "READY";
            case Command.GetGameState:
                return "GET_GAME_STATE";
            case Command.GameStateResponse:
                return "GAME_STATE_RESPONSE";
            case Command.MalformedCommand:
                return "MALFORMED_COMMAND";
            case Command.GetImage:
                return "GET_IMAGE";
            case Command.GetImageResponse:
                return "IMAGE";
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
        else if (commandString.Equals("PAUSE"))
        {
            return Command.Pause;
        } else if(commandString.Equals("READY")) 
        {
            return Command.Ready;
        }
        else if (commandString.Equals("GET_GAME_STATE"))
        {
            return Command.GetGameState;
        } 
        else if(commandString.Equals("GAME_STATE")) 
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
        else if(commandString.Equals("GET_IMAGE")) {
            return Command.GetImage;
        }
        else if (commandString.Equals("IMAGE"))
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
        else
        {
            return Command.NoCommand;
        }
    }
}