mergeInto(LibraryManager.library, {
	connectToSocket: function(socket) {

		window.WebSocket = window.WebSocket || window.MozWebSocket;
		window.socket = new WebSocket(Pointer_stringify(socket));
			
		window.socket.onmessage = function (message) {
			if(message.data != null) {
				unityInstance.SendMessage('NetworkManager', 'AddToQueue', message.data);
			}
		};
	},
	
	sendMessage: function(msg) {
		if(window.socket != null && window.socket.readyState === 1){
			window.socket.send(Pointer_stringify(msg));
		}
	},

	closeSocket: function() {
		window.socket.close();
		window.socker = null;
	}
});

