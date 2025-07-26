mergeInto(LibraryManager.library, {
  Telegram_PostScore: function (score) {
    if (typeof TelegramGameProxy !== 'undefined') {
      TelegramGameProxy.postScore(score);
    }
  }
});