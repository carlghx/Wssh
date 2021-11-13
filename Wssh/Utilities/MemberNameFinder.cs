/// Taken from http://code.google.com/p/gim-projects/source/browse/presentations/CantDanceTheLambda/src/MemberNameParser.cs

// Author: George Mauer
// Code samples for presentation You Can't Dance the Lambda
// Slide: And More!
// Build a class to provide access to names of class members without ever having to use weakly typed strings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Wssh.Utilities
{
  /// <summary>
  /// This class allows you to read a class's property names
  /// </summary>
  /// <typeparam name="ClassType">Class to find properties of</typeparam>
  public static class MemberNameFinder<ClassType>
  {
    /// <summary>
    /// Use reflection and lambda expressions to get the name of a property in a class
    /// There has to be (or should be) an easier way to do this ...    
    /// </summary>
    /// <typeparam name="ReturnType">Property's return type</typeparam>
    /// <param name="expression">Lambda expression of the form x => x.Property</param>
    /// <returns>Name of the property specified in expr</returns>
    public static string GetMemberName<ReturnType>(Expression<Func<ClassType, ReturnType>> expression)
    {
      var node = expression.Body as MemberExpression;
      if (object.ReferenceEquals(null, node))
        throw new InvalidOperationException("Expression must be of member access");
      return node.Member.Name;
    }       

  }
}
