package com.cyberland.voice;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.speech.RecognitionListener;
import android.speech.RecognizerIntent;
import android.speech.SpeechRecognizer;

import com.unity3d.player.UnityPlayer;

import java.util.ArrayList;
import java.util.Locale;

public class VoiceRecognizerPlugin {
    private static SpeechRecognizer speechRecognizer;
    private static Intent recognizerIntent;
    private static String unityObjectName;

    public void startListening(Activity activity, String receiverObjectName) {
        unityObjectName = receiverObjectName;

        activity.runOnUiThread(() -> {
            try {
                if (!SpeechRecognizer.isRecognitionAvailable(activity)) {
                    UnityPlayer.UnitySendMessage(unityObjectName, "OnVoiceError", "Speech recognition not available on this device.");
                    return;
                }

                if (speechRecognizer == null) {
                    speechRecognizer = SpeechRecognizer.createSpeechRecognizer(activity);

                    speechRecognizer.setRecognitionListener(new RecognitionListener() {
                        @Override public void onReadyForSpeech(Bundle params) {}
                        @Override public void onBeginningOfSpeech() {}
                        @Override public void onRmsChanged(float rmsdB) {}
                        @Override public void onBufferReceived(byte[] buffer) {}
                        @Override public void onEndOfSpeech() {}

                        @Override
                        public void onError(int error) {
                            UnityPlayer.UnitySendMessage(unityObjectName, "OnVoiceError", "Speech error code: " + error);
                        }

                        @Override
                        public void onResults(Bundle results) {
                            ArrayList<String> matches =
                                results.getStringArrayList(SpeechRecognizer.RESULTS_RECOGNITION);

                            if (matches != null && !matches.isEmpty()) {
                                UnityPlayer.UnitySendMessage(unityObjectName, "OnVoiceResult", matches.get(0));
                            } else {
                                UnityPlayer.UnitySendMessage(unityObjectName, "OnVoiceError", "No speech match");
                            }
                        }

                        @Override public void onPartialResults(Bundle partialResults) {}
                        @Override public void onEvent(int eventType, Bundle params) {}
                    });
                }

                if (recognizerIntent == null) {
                    recognizerIntent = new Intent(RecognizerIntent.ACTION_RECOGNIZE_SPEECH);
                    recognizerIntent.putExtra(
                        RecognizerIntent.EXTRA_LANGUAGE_MODEL,
                        RecognizerIntent.LANGUAGE_MODEL_FREE_FORM
                    );
                    recognizerIntent.putExtra(
                        RecognizerIntent.EXTRA_LANGUAGE,
                        Locale.getDefault()
                    );
                    recognizerIntent.putExtra(
                        RecognizerIntent.EXTRA_MAX_RESULTS,
                        3
                    );
                }

                speechRecognizer.startListening(recognizerIntent);

            } catch (Exception e) {
                UnityPlayer.UnitySendMessage(unityObjectName, "OnVoiceError", e.getMessage());
            }
        });
    }

    public void stopListening() {
        if (speechRecognizer != null) {
            speechRecognizer.stopListening();
        }
    }

    public void destroy() {
        if (speechRecognizer != null) {
            speechRecognizer.destroy();
            speechRecognizer = null;
        }
    }
}