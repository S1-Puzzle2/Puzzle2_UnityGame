using UnityEngine;
using System.Collections;

public enum Command {
	testCommand
}

public class TransferObject {

	public Command Command {
		get; private set;
	}

	public System.Object Data {
		get; private set;
	}
	
	public TransferObject(Command command, System.Object data) {
		this.Command = command;
		this.Data = data;
	}


}
