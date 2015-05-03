function CoffeeOrderViewModel(user) {
	var self = this;

	// api client
	var client = {
		get: function(callback) {
			$.ajax({
				url: '/api/coffee',
				success: callback
			});
		},

		add: function (kind, pack, onSuccess) {
			$.ajax({
				url: '/api/coffee/add?coffeeName=' + kind.name() + '&weight=' + pack.weight(),
				method: 'POST',
				success: onSuccess
			});
		},

		remove: function (kind, pack, onSuccess) {
			$.ajax({
				url: '/api/coffee/remove?coffeeName=' + kind.name() + '&weight=' + pack.weight(),
				method: 'POST',
				success: onSuccess
			});
		},
	};

	// VM declaration
	$.extend(self, {
		user: user,
		availableCoffeeKinds: ko.observableArray(),
		nonAvailableCoffeeKinds: ko.observableArray(),

		availablePacks: ko.computed(function () {
			if (self.availableCoffeeKinds().length == 0) {
				return [];
			}

			return self.availableCoffeeKinds()[0].packs();
		}, self, { deferEvaluation: true }),
	});

	function updateCoffeeKindsFromServer() {
		self.availableCoffeeKinds([]);
		self.nonAvailableCoffeeKinds([]);

		client.get(function (coffeeKinds) {
			for (var i = 0; i < coffeeKinds.length; i++) {
				if (coffeeKinds[i].isAvailable) {
					var child = ko.mapping.fromJS(coffeeKinds[i], {}, new coffeeKindViewModel());
					self.availableCoffeeKinds.push(child);
				} else {
					self.nonAvailableCoffeeKinds.push(coffeeKinds[i]);
				}
			}
		});
	}

	function coffeeKindViewModel() {
		var self = this;
		$.extend(this, {
			isOrdered: ko.computed(function() {
				for (var i = 0; i < self.packs().length; i++) {
					if (self.packs()[i].count() > 0) {
						return true;
					}
				}

				return false;
			}, self, { deferEvaluation: true }),

			add: function(pack) {
				client.add(self, pack, function() {
					pack.count(pack.count() + 1);
				});
			},

			remove: function(pack) {
				client.remove(self, pack, function () {
					pack.count(pack.count() - 1);
				});
			}
		});
	}

	// init
	updateCoffeeKindsFromServer();
}