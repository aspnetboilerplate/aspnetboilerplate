### Introduction

Most **SaaS** (multi-tenant) applications have **editions** (packages)
those have different **features**. Thus, they can provide different
**price and feature options** to thier tenants (customers).

#### About Features

See [feature management
documentation](/Pages/Documents/Feature-Management) to understand
features.

### Edition Entity

**Edition** is a simple entity represents an edition (or package) of the
application. It has just **Name** and **DisplayName** properties.

### Edition Manager

**EditionManager** is the **domain service** to manage editions:

    public class EditionManager : AbpEditionManager
    {
    }

It's derived from **AbpEditionManager** class. You can inject and use
EditionManager to create, delete, update editions. Also, EditionManager
is used to **manage features** of editions. It internally **caches**
edition features for better performance.
