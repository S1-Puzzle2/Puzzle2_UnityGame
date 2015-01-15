
public enum Command {
	Register,
	PuzzleFinished,
	Ready,
	Pause,
	QrCodeSend,
	GetGameState,
    GameStateResponse,
	PieceScanned,
	PenaltyTimeAdd,
	AreYouThere,
    NoCommand,
    MalformedCommand,
    Registered
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
            case Command.Registered:
                return "REGISTERED";
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
        else if(commandString.Equals("GAME_STATE_RESPONSE")) 
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
        else
        {
            return Command.NoCommand;
        }
    }
}