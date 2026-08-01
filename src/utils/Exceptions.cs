// Unminal Engine - Copyright (C) 2026 Dov1ntc
// Licensed under GNU AGPLv3 with No-Misattribution Addendum
// See LICENSE file for details.
public class Crash: Exception {
    public Crash(): base(){}
    public Crash(string message): base(message){}
    public Crash(string message, Exception exception): base(message, exception){}
}
