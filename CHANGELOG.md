0.1.6 (2017-02-09)
-------------------
- Removed IDisposable implementations (no need for them).
- Improved EventAggregator implementation by removing internal static event in favor of weak references to target objects.
- Write speeds have been doubly improved as a result of these changes.

0.1.5 (2017-02-06)
-------------------
- Implemented EventAggregator.

0.1.4 (2017-01-23)
-------------------
- Compatible with Realm version 0.82.0.
- AutoIncrementer is now a single instance.
- Implemented IDisposable on RealmServiceBase and derived class.

0.1.3 (2017-11-01)
-------------------
- Specified dependencies in nuspec.

0.1.2 (2017-11-01)
-------------------
- Missing dependency on Realm.

0.1.1 (2017-11-01)
-------------------
- Removed unused reference (reoccurring issue).

0.1.0 (2017-11-01)
-------------------
- Initial release.
