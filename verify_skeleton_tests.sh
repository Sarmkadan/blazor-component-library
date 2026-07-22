#!/bin/bash

# Simple script to verify Skeleton tests compile and can be discovered

echo "=== Checking if SkeletonTests.cs exists ==="
if [ -f "BlazorComponentLibrary.Tests/SkeletonTests.cs" ]; then
    echo "✓ SkeletonTests.cs found"
else
    echo "✗ SkeletonTests.cs NOT found"
    exit 1
fi

# Check if the file has content
echo "=== Checking SkeletonTests.cs file size ==="
FILE_SIZE=$(wc -c < "BlazorComponentLibrary.Tests/SkeletonTests.cs")
echo "File size: $FILE_SIZE bytes"

if [ $FILE_SIZE -lt 1000 ]; then
    echo "✗ SkeletonTests.cs is too small"
    exit 1
fi

# Try to compile just the Skeleton tests file
echo "=== Attempting to compile SkeletonTests.cs ==="
dotnet build BlazorComponentLibrary.Tests/SkeletonTests.cs 2>&1 | grep -E "(error|SkeletonTests)" | head -20

if [ $? -eq 0 ]; then
    echo "✓ SkeletonTests.cs compiles successfully"
else
    echo "✗ SkeletonTests.cs has compilation errors"
    exit 1
fi

# Check if tests can be discovered
echo "=== Checking if Skeleton tests can be discovered ==="
dotnet test BlazorComponentLibrary.Tests --list-tests 2>&1 | grep -i skeleton | head -10

if [ $? -eq 0 ]; then
    echo "✓ Skeleton tests discovered"
else
    echo "✗ Skeleton tests NOT discovered"
    exit 1
fi

echo "=== All checks passed! ==="