mergeInto(LibraryManager.library, {
  ConnectWalletJS: function () {
    console.log("✅ ConnectWalletJS called");
    // This is where you will add actual wallet connection logic later
  },

  GetWalletAddressJS: function () {
    const address = "EQC123456789ABCDEF"; // dummy address
    const buffer = allocate(intArrayFromString(address), 'i8', ALLOC_NORMAL);
    return buffer;
  },

  PostScoreJS: function (score) {
    console.log("✅ Score posted:", score);
  },

  SendRewardJS: function (amount) {
    console.log("✅ Sending reward:", amount);
  }
});