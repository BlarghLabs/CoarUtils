namespace CoarUtils.commands.web {
  /// <summary>
  /// What the X-Forwarded-For header a caller sent says about its intent.
  ///
  /// Our load balancer appends exactly one entry (the real client address) to whatever arrived. So the shape of
  /// the chain tells us what the caller did:
  ///   none          - one entry, and it parses. The caller sent no forwarding header of its own; the only
  ///                   entry is the one our proxy wrote. This is what ordinary traffic looks like.
  ///   clientSupplied- more than one entry, all parseable. The caller sent its own forwarding header. Worth
  ///                   recording, but has innocent explanations (a corporate proxy, a CDN in front of us).
  ///   forged        - an entry that cannot be an IP address at all. No legitimate client does this; it is a
  ///                   deliberate attempt to poison attribution, and is treated as an attack rather than as
  ///                   reconnaissance.
  /// </summary>
  public enum ForwardedHeaderSpoofVerdict {
    none,
    clientSupplied,
    forged,
  }

  public static class ForwardedHeaderSpoofVerdictExtensions {
    /// <summary>
    /// The value persisted inside the forwarded_headers JSON. Kept distinct from the member name so the stored
    /// form stays snake_case and stable even if the enum member is renamed — downstream detection matches on
    /// these strings.
    /// </summary>
    public static string GetLabel(this ForwardedHeaderSpoofVerdict forwardedHeaderSpoofVerdict) {
      switch (forwardedHeaderSpoofVerdict) {
        case ForwardedHeaderSpoofVerdict.clientSupplied: return "client_supplied";
        case ForwardedHeaderSpoofVerdict.forged: return "forged";
        case ForwardedHeaderSpoofVerdict.none: return "none";
        default: return "unk";
      }
    }
  }
}
