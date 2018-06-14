### Introduction

Most **SaaS** (multi-tenant) applications have **editions** (packages)
that have different **features**. This way, they can provide different
**price and feature options** to their tenants (customers).

#### About Features

See the [feature management
documentation](/Pages/Documents/Feature-Management) to better understand
how features work.

### Edition Entity

**Edition** is a simple entity representing an edition (or package) of the
application. It just has the **Name** and **DisplayName** properties.

### Edition Manager

**EditionManager** is the **domain service** to manage editions:

    public class EditionManager : AbpEditionManager
    {
    }

It's derived from the **AbpEditionManager** class. You can inject and use
the EditionManager to create, delete, and update editions. EditionManager
is also used to **manage the features** of editions. It internally **caches**
edition features for better performance.
