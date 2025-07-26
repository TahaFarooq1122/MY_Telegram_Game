mergeInto(LibraryManager.library, {
  TON_ConnectWallet: function () {
    if (typeof TonWallet !== 'undefined') {
      TonWallet.connect();
    }
  },
  TON_SendReward: function (tonAddressPtr) {
    var tonAddress = UTF8ToString(tonAddressPtr);
    if (typeof TonWallet !== 'undefined') {
      TonWallet.sendReward(tonAddress);
    }
  }
});