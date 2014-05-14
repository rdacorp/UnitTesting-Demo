/*global todomvc */
'use strict';

/**
 * Services that persists and retrieves TODOs from localStorage
 */
todomvc.factory('todoStorage', function ($http) {
	return {
		getAsync: function (callback) {
			$http.get("/todo").success(callback);
		},

		addAsync: function (todo, callback) {
			$http.post("/todo", todo).success(callback);
		},

		updateAsync: function (todo, callback) {
			$http.put("/todo/" + todo.id, todo).success(callback);
		},

		deleteAsync: function (todo, callback) {
			if (!todo.id) {
				callback();
				return;
			}

			$http.delete("/todo/" + todo.id).success(callback);
		},

		clearCompletedAsync: function(callback) {
			$http.post("/todo/clearCompleted").success(callback);
		}
	};
});
