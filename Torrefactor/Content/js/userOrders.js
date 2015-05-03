function UserOrderViewModel(user) {

	var self = this;

	// API client
	var client = {
		get: function (success) {
			$.ajax({
				url: '/api/users/orders',
				success: success
			});
		},
		
		reload: function(callback) {
			$.ajax({
				url: '/api/coffee/reload',
				method: 'POST',
				success: function () { callback(true); },
				error: function () { callback(false); },
			});
		},

		upload: function (callback) {
			$.ajax({
				url: '/api/coffee/send',
				method: 'POST',
				success: function () { callback(true); },
				error: function () { callback(false); },
			});
		},

		clear: function (callback) {
			$.ajax({
				url: '/api/coffee/clear',
				method: 'POST',
				success: function () { callback(true); },
				error: function () { callback(false); },
			});
		}
	};

	// VM declaration
	$.extend(self, {
		user: user,

		orders: ko.observableArray(),

		isLoading: ko.observable(false),

		overallPrice: ko.computed(function () {
			var res = 0;
			for (var i = 0; i < self.orders().length; i++) {
				res += self.orders()[i].price();
			}
			return res;
		}, self, { deferEvaluation: true }),

		reload: function () {
			self.isLoading(true);
			client.reload(finishLoading);
		},

		upload: function () {
			self.isLoading(true);
			client.upload(finishLoading);
		},

		clear: function () {
			self.isLoading(true);
			client.clear(finishLoading);
		}
	});

	// init
	function reload() {
		self.orders([]);
		client.get(function (orders) {
			for (var i = 0; i < orders.length; i++) {
				self.orders.push(ko.mapping.fromJS(orders[i]));
			}
		});
	}

	function finishLoading(res) {
		self.isLoading(false);
		if (!res) {
			alert('Smth has really gone wrong...');
		} else {
			reload();
		}
	}

	reload();
}