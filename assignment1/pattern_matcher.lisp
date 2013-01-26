;;; pattern_matcher.lisp
;;; a simple pattern matcher

(defun match (pattern data)

	; base case: pattern is empty
	(when (= 0 (length pattern))
		(if (= 0 (length data))
			(return-from match t)
			(return-from match NIL)))

	; base case: data is empty
	(when (= 0 (length data))
		(if (equal (first pattern) '*)
			(return-from match (match (rest pattern) data))
			(return-from match NIL)))

	(let ((p (first pattern)) (d (first data)))
		
		(cond
			; if p and d are equal or p is ?, recurse
			((or (equal p d) (equal p '?))
				(match (rest pattern) (rest data)))
	
			; else if p is *, recurse twice and merge results
			((equal p '*)
				(let ((noData (match (rest pattern) data))
					 (withData (match pattern (rest data))))
					(cond
						; if one result is "NIL", return the other
						((equal noData NIL)
							withData)
						((equal withData NIL)
							noData)
						; if both results are "t", return "t"
						((and (equal noData t) (equal withData t))
							t)
						; if either result is "t", return the other
						((equal noData t)
							withData)
						((equal withData t)
							noData)
						; if we already have a list of association lists, just append
						((listp (first (first withData)))
							(append (list noData) withData))
						((listp (first (first noData)))
							(append noData (list withData)))
						; both are association lists, make a list of association lists
						((list noData withData)))))

			; else if p starts with "?", recurse and add to list
			((and (symbolp p) (equal (subseq (symbol-name p) 0 1) "?"))
				(let ((subResult (match (rest pattern) (rest data)))
					 (newAssoc (list (list p d))))
					(cond
						((equal subResult NIL)
							NIL)
						((equal subResult t)
							newAssoc)
						((append subResult newAssoc)))))

			; else no match
			(NIL))))

(defun test-match ()
	(test-one-match '() '() t)
	(test-one-match '(ai) '(ai) t)
	(test-one-match '(ai cs) '(ai cs) t)
	(test-one-match '(cs ai) '(ai cs) NIL)
	(test-one-match '(1 2 3 0) '(1 2 3 4 0) NIL)
	(test-one-match '(? mudd) '(seely mudd) t)
	(test-one-match '(?first ?middle mudd) '(seely w mudd) '((?middle w) (?first seely)))
	(test-one-match '(? ?x ? ?y ?) '(Warren Buffet Is A Good Man) NIL)
	(test-one-match '(School Of Engineering and Applied Science) '(School Of Engineering) NIL)
	(test-one-match '(* School Of Engineering and Applied Science) '(The Fu Foundation School Of Engineering and Applied Science) t)
	(test-one-match '(The * School Of Engineering and Applied Science) '(The Fu Foundation School Of Engineering and Applied Science) t)
	(test-one-match '(The * School Of Engineering and Applied Science) '(The School Of Engineering and Applied Science) t)
	(test-one-match '(* 3 ?x 4 *) '(3 5 4) '((?x 5)))
	(test-one-match '(?x (1 2) ?y (4 5)) '(c (1 2) d (4 5)) '((?y d)(?x c)))
	(test-one-match '(?y ?z (c v)) '(8 gh (c v) ) '((?z gh)(?y 8)))
	(test-one-match '(((get) me) out) '(get (me (out))) NIL)
	(test-one-match '(A * B) '(A A A A A B) t)
	(test-one-match '(?x * ?y) '(A A A A A B) '((?y b)(?x a)))
	(test-one-match '(? ? ?) '(a (a b) ((a b c))) t)
	(test-one-match '(a * ?x *) '(a b c d) '(((?x b)) ((?x c)) ((?x d))))
	"DONE!")

(defun test-one-match (pattern data output)
	
	; log the inputs
	(print (format NIL "pattern: ~a" pattern))
	(print (format NIL "data:    ~a" data))

	; check the output
	(let ((result (match pattern data)))
		(if (equal result output)
			(print "TEST PASSED")
			(progn
				(print "******** TEST FAILED ********")
				(print (format NIL "expected: ~a" output))
				(print (format NIL "result:   ~a" result)))))

	; skip a line
	(print ""))


